using System;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR && GES_MVC_PROFILING
using Unity.Profiling;
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
            FieldInfo[] fields = resolving.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            
        #if UNITY_EDITOR && GES_MVC_PROFILING
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
            
        #if UNITY_EDITOR && GES_MVC_PROFILING
            resolvingMarker.End();
        #endif
        }
    }
}