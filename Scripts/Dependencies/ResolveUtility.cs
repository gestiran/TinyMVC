using System;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Text;
using Unity.Profiling;
using TinyMVC.Debugging;
#endif

namespace TinyMVC.Dependencies {
    internal static class ResolveUtility {
        private static readonly Type _injectType;
        
        static ResolveUtility() => _injectType = typeof(InjectAttribute);

        internal static void ResolveWithoutApply(IResolving resolving, DependencyContainer container) {
            Resolve(resolving, container.dependencies, _injectType);
        }
        
        internal static void ResolveWithoutApply(List<IResolving> resolving, DependencyContainer container) {
            Resolve(resolving, container.dependencies, _injectType);
        }
        
        internal static void Resolve(IResolving resolving, object owner, DependencyContainer container) {
            Resolve(resolving, container.dependencies, _injectType);
            TryApply(resolving, owner);
        }

        internal static void Resolve(List<IResolving> resolving, object owner, DependencyContainer containers) {
            Resolve(resolving, containers.dependencies, _injectType);
            TryApply(resolving, owner);
        }

        private static void TryApply(List<IResolving> resolving, object owner) {
            for (int resolvingId = 0; resolvingId < resolving.Count; resolvingId++) {
                TryApply(resolving[resolvingId], owner);
            }
        }
        
        private static void TryApply(IResolving resolving, object owner) {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            ValidateFields(resolving, _injectType, owner);
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

        private static void ValidateFields(IResolving resolving, Type injectType, object owner) {
            FieldInfo[] fields = GetFields(resolving);
            
            for (int fieldId = 0; fieldId < fields.Length; fieldId++) {
                if (!Attribute.IsDefined(fields[fieldId], injectType)) {
                    continue;
                }

                InjectAttribute inject = fields[fieldId].GetCustomAttribute<InjectAttribute>();

                if (!inject.isRequired) {
                    continue;
                }

                if (fields[fieldId].GetValue(resolving) != null) {
                    continue;
                }
                
                if (resolving is UnityEngine.Object context) {
                    UnityEngine.Debug.LogError(Log(resolving, fields[fieldId], owner).Bold(), context);
                } else {
                    UnityEngine.Debug.LogError(Log(resolving, fields[fieldId], owner).Bold());
                }
            }
        }

        private static string Log(IResolving resolving, FieldInfo field, object owner) {
            string access = field.IsPrivate ? "private" : "protected";
            string ownerName = owner == null ? "" : $" {owner.GetType().Name}";
            return $"Resolve{ownerName} {DebugUtility.Link(resolving)} required [{nameof(InjectAttribute)}] {access} {LogField(field)} {field.Name}";
        }

        private static string LogField(FieldInfo field) {
            if (field.FieldType.IsGenericType) {
                StringBuilder builder = new StringBuilder(4);

                builder.Append(field.FieldType.Name[..^2]);
                builder.Append("<");
                
                Type[] generic = field.FieldType.GenericTypeArguments;

                if (generic.Length > 0) {
                    int i = 0;
                
                    while (true) {
                        builder.Append(DebugUtility.Link(generic[i].Name));
                    
                        i++;
                    
                        if (i < generic.Length) {
                            builder.Append(", ");
                        } else {
                            break;
                        }
                    }
                }
                
                builder.Append(">");
                
                return builder.ToString();
            }

            return DebugUtility.Link(field.FieldType.Name);
        }

    #endif
    }
}