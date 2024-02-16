using System;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Unity.Profiling;
using TinyMVC.Exceptions;
#endif

namespace TinyMVC.Dependencies {
    internal static class ResolveUtility {
        private static readonly Type _injectType;
        
        static ResolveUtility() => _injectType = typeof(Inject);

        internal static void ResolveWithoutApply(IResolving resolving, DependencyContainer container) {
            Resolve(resolving, container.dependencies, _injectType);
        }
        
        internal static void ResolveWithoutApply(List<IResolving> resolving, DependencyContainer container) {
            Resolve(resolving, container.dependencies, _injectType);
        }
        
        internal static void Resolve(IResolving resolving, DependencyContainer container) {
            Resolve(resolving, container.dependencies, _injectType);
            TryApply(resolving);
        }

        internal static void Resolve(List<IResolving> resolving, DependencyContainer containers) {
            Resolve(resolving, containers.dependencies, _injectType);
            TryApply(resolving);
        }

        private static void TryApply(List<IResolving> resolving) {
            for (int resolvingId = 0; resolvingId < resolving.Count; resolvingId++) {
                TryApply(resolving[resolvingId]);
            }
        }
        
        private static void TryApply(IResolving resolving) {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            ValidateFields(resolving, _injectType);
        #endif
            
            if (resolving is IApplyResolving applyResolving) {
                applyResolving.ApplyResolving();
            }
        }
        
        private static void Resolve(List<IResolving> resolving, Dictionary<Type, IDependency> dependencies, Type injectType) {
            for (int resolvingId = 0; resolvingId < resolving.Count; resolvingId++) {
                Resolve(resolving[resolvingId], dependencies, injectType);
            }
        }

        private static void Resolve(IResolving resolving, Dictionary<Type, IDependency> dependencies, Type injectType) {
            FieldInfo[] fields = GetFields(resolving);
            
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            ProfilerMarker resolvingMarker = new ProfilerMarker(ProfilerCategory.Scripts, $"Resolve(Resolving: {resolving.GetType().Name})");
            resolvingMarker.Begin();
        #endif
            
            for (int fieldId = 0; fieldId < fields.Length; fieldId++) {
                if (!Attribute.IsDefined(fields[fieldId], injectType)) {
                    continue;
                }

                Type fieldType = fields[fieldId].FieldType;

                if (!dependencies.ContainsKey(fieldType)) {
                    continue;
                }

                fields[fieldId].SetValue(resolving, dependencies[fieldType]);
            }
            
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            resolvingMarker.End();
        #endif
        }

        private static FieldInfo[] GetFields(IResolving resolving) {
            return resolving.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        }

    #if UNITY_EDITOR || DEVELOPMENT_BUILD

        private static void ValidateFields(IResolving resolving, Type injectType) {
            FieldInfo[] fields = GetFields(resolving);
            
            for (int fieldId = 0; fieldId < fields.Length; fieldId++) {
                if (!Attribute.IsDefined(fields[fieldId], injectType)) {
                    continue;
                }

                Inject inject = fields[fieldId].GetCustomAttribute<Inject>();

                if (!inject.isRequired) {
                    continue;
                }

                if (fields[fieldId].GetValue(resolving) != null) {
                    continue;
                }
                
                if (resolving is UnityEngine.Object context) {
                    UnityEngine.Debug.LogError(Log(resolving, fields[fieldId]), context);
                } else {
                    UnityEngine.Debug.LogError(Log(resolving, fields[fieldId]));
                }
            }
        }

        private static string Log(IResolving resolving, FieldInfo field) {
            string access = field.IsPrivate ? "private" : "protected";
            return $"Resolve {LogUtility.Link(resolving)} required [{nameof(Inject)}] {access} {field.FieldType.Name} {field.Name}";
        }

    #endif
    }
}