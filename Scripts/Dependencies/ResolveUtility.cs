using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TinyMVC.Boot;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Text;
#endif

namespace TinyMVC.Dependencies {
    internal static class ResolveUtility {
        private static readonly Type _injectType;
        private static readonly Type _groupType;
        private static readonly Dictionary<Type, FieldData[]> _cache;
        
        private readonly struct FieldData {
            public readonly FieldInfo info;
            public readonly string group;
            
            public FieldData(FieldInfo info, string group) {
                this.info = info;
                this.group = group;
            }
        }
        
        static ResolveUtility() {
            _injectType = typeof(InjectAttribute);
            _groupType = typeof(ResolveGroupAttribute);
            _cache = new Dictionary<Type, FieldData[]>(256);
        }
        
        internal static void Resolve(IResolving resolving, DependencyContainer container) {
            Resolve(resolving, container, _injectType, _groupType);
        }
        
        internal static void Resolve(IResolving resolving) {
            Resolve(resolving, DependencyContainer.empty, _injectType, _groupType);
        }
        
        internal static void Resolve(List<IResolving> resolving, DependencyContainer container) {
            Resolve(resolving, container, _injectType, _groupType);
        }
        
        internal static void Resolve(List<IResolving> resolving) {
            Resolve(resolving, DependencyContainer.empty, _injectType, _groupType);
        }
        
        internal static void Resolve(IResolving[] resolving, DependencyContainer container) {
            Resolve(resolving, container, _injectType, _groupType);
        }
        
        internal static void Resolve(IResolving[] resolving) {
            Resolve(resolving, DependencyContainer.empty, _injectType, _groupType);
        }
        
        internal static void TryApply(List<IResolving> resolving) {
            for (int resolvingId = 0; resolvingId < resolving.Count; resolvingId++) {
                TryApply(resolving[resolvingId]);
            }
        }
        
        internal static void TryApply(IResolving[] resolving) {
            for (int resolvingId = 0; resolvingId < resolving.Length; resolvingId++) {
                TryApply(resolving[resolvingId]);
            }
        }
        
        internal static void TryApply(IResolving resolving) {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            ValidateFields(resolving, _injectType);
        #endif
            
            if (resolving is IApplyResolving applyResolving) {
                applyResolving.ApplyResolving();
            }
        }
        
        private static void Resolve(List<IResolving> resolving, DependencyContainer container, Type injectType, Type groupType) {
            for (int resolvingId = 0; resolvingId < resolving.Count; resolvingId++) {
                Resolve(resolving[resolvingId], container, injectType, groupType);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Resolve(IResolving[] resolving, DependencyContainer container, Type injectType, Type groupType) {
            for (int resolvingId = 0; resolvingId < resolving.Length; resolvingId++) {
                Resolve(resolving[resolvingId], container, injectType, groupType);
            }
        }
        
        private static void Resolve(IResolving resolving, DependencyContainer container, Type injectType, Type groupType) {
            Type resolvingType = resolving.GetType();
            
            if (_cache.TryGetValue(resolvingType, out FieldData[] data)) {
                for (int fieldId = 0; fieldId < data.Length; fieldId++) {
                    Type fieldType = data[fieldId].info.FieldType;
                    
                    if (container.dependencies.TryGetValue(fieldType, out IDependency dependency)) {
                        data[fieldId].info.SetValue(resolving, dependency);
                        continue;
                    }
                    
                    if (ProjectContext.data.TryGetDependency(data[fieldId].group, fieldType, out dependency)) {
                        data[fieldId].info.SetValue(resolving, dependency);
                    }
                }
            } else {
                FieldInfo[] fields = GetFields(resolvingType);
                List<FieldData> result = new List<FieldData>(fields.Length);
                
                for (int fieldId = 0; fieldId < fields.Length; fieldId++) {
                    if (Attribute.IsDefined(fields[fieldId], injectType) == false) {
                        continue;
                    }
                    
                    Type fieldType = fields[fieldId].FieldType;
                    
                    ResolveGroupAttribute attribute = (ResolveGroupAttribute)Attribute.GetCustomAttribute(fieldType, groupType);
                    string group = attribute != null ? attribute.group : ProjectData.MAIN;
                    
                    result.Add(new FieldData(fields[fieldId], group));
                    
                    if (container.dependencies.TryGetValue(fieldType, out IDependency dependency)) {
                        fields[fieldId].SetValue(resolving, dependency);
                        continue;
                    }
                    
                    if (ProjectContext.data.TryGetDependency(group, fieldType, out dependency)) {
                        fields[fieldId].SetValue(resolving, dependency);
                    }
                }
                
                _cache.Add(resolvingType, result.ToArray());
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FieldInfo[] GetFields(Type resolving) {
            return resolving.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        }
        
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        
        // Resharper disable Unity.ExpensiveCode
        private static void ValidateFields(IResolving resolving, Type injectType) {
            FieldInfo[] fields = GetFields(resolving.GetType());
            
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
                    UnityEngine.Debug.LogError(Log(resolving, fields[fieldId]), context);
                } else {
                    UnityEngine.Debug.LogError(Log(resolving, fields[fieldId]));
                }
            }
        }
        
        private static string Log(IResolving resolving, FieldInfo field) {
            string access = field.IsPrivate ? "private" : "protected";
            
            return $"Resolve {resolving} required [{nameof(InjectAttribute)}] {access} {LogField(field)} {field.Name}";
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
                        builder.Append(generic[i].Name);
                        
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
            
            return field.FieldType.Name;
        }
        
    #endif
    }
}