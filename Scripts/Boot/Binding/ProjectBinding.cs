using System;
using System.Collections.Generic;
using TinyMVC.Dependencies;
using TinyMVC.Dependencies.Extensions;

namespace TinyMVC.Boot.Binding {
    public static class ProjectBinding {
        private static readonly Dictionary<Type, List<RequirementsGroup>> _binders;
        
        static ProjectBinding() => _binders = new Dictionary<Type, List<RequirementsGroup>>(32);
        
        private sealed class RequirementsGroup {
            public readonly Binder binder;
            public readonly Type[] requirements;
            
            public RequirementsGroup(Binder binder, Type[] requirements) {
                this.binder = binder;
                this.requirements = requirements;
            }
        }
        
        public static bool TryBind<T>(out T model) where T : IDependency, new() {
            if (TryFindBinder(Array.Empty<IDependency>(), out Binder<T> binder)) {
                ResolveUtility.Resolve(binder);
                model = binder.Bind();
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool TryBind<T>(string key, out T model) where T : IDependency, new() {
            if (TryFindBinder(Array.Empty<IDependency>(), out Binder<T> binder)) {
                binder.keyValue = key;
                ResolveUtility.Resolve(binder);
                model = binder.Bind();
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool TryBind<T>(out T model, params IDependency[] dependencies) where T : IDependency, new() {
            if (TryFindBinder(dependencies, out Binder<T> binder)) {
                ResolveUtility.Resolve(binder, new DependencyContainer(dependencies));
                model = binder.Bind();
                return true;
            }
            
            model = default;
            return false;
        }
        
        public static bool TryBind<T>(out T model, string key, params IDependency[] dependencies) where T : IDependency, new() {
            if (TryFindBinder(dependencies, out Binder<T> binder)) {
                binder.keyValue = key;
                ResolveUtility.Resolve(binder, new DependencyContainer(dependencies));
                model = binder.Bind();
                return true;
            }
            
            model = default;
            return false;
        }
        
        internal static void Add<T>(T binder) where T : Binder {
            Type type = binder.GetBindType();
            Type binderType = typeof(T);
            RequirementsGroup group = new RequirementsGroup(binder, DependencyUtility.GetRequirements(typeof(T)));
            
            if (_binders.TryGetValue(type, out List<RequirementsGroup> groups)) {
                for (int groupId = 0; groupId < groups.Count; groupId++) {
                    if (groups[groupId].binder.GetType() != binderType) {
                        continue;
                    }
                    
                    groups[groupId] = group;
                    return;
                }
                
                groups.Add(group);
            } else {
                _binders.Add(type, new List<RequirementsGroup>(8) { group });
            }
        }
        
        internal static void Remove<T>(T binder) where T : Binder => _binders.Remove(binder.GetBindType());
        
        private static bool TryFindBinder<T>(IDependency[] dependencies, out Binder<T> binder) where T : IDependency, new() {
            Type[] references = dependencies.AsRequirements();
            
            if (_binders.TryGetValue(typeof(T), out List<RequirementsGroup> groups)) {
                foreach (RequirementsGroup group in groups) {
                    if (CheckRequirements(group.requirements, references)) {
                        binder = (Binder<T>)group.binder;
                        return true;
                    }
                }
            }
            
            binder = null;
            return false;
        }
        
        private static bool CheckRequirements(Type[] requirements, Type[] dependencies) {
            for (int requirementId = 0; requirementId < requirements.Length; requirementId++) {
                if (IsContain(dependencies, requirements[requirementId])) {
                    continue;
                }
                
                return false;
            }
            
            return true;
        }
        
        private static bool IsContain(Type[] types, Type type) {
            for (int typeId = 0; typeId < types.Length; typeId++) {
                if (types[typeId] == type) {
                    return true;
                }
            }
            
            return false;
        }
    }
}