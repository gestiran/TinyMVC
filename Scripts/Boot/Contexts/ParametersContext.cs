using System.Collections.Generic;
using TinyMVC.Dependencies;
using UnityEngine;

namespace TinyMVC.Boot.Contexts {
    public abstract class ParametersContext {
        internal readonly List<IDependency> all;
        
        public sealed class EmptyContext : ParametersContext {
            internal EmptyContext() { }
            
            protected override void Create() { }
        }
        
        protected ParametersContext() => all = new List<IDependency>();
        
        public static EmptyContext Empty() => new EmptyContext();
        
        internal void Init() => Create();
        
        internal void AddDependencies(List<IDependency> dependencies) => dependencies.AddRange(all);
        
        protected abstract void Create();
        
        protected void Add<T>(T dependency) where T : ScriptableObject, IDependency {
        #if UNITY_EDITOR
            
            if (dependency == null) {
                Debug.LogError($"Can't find {typeof(T).Name} parameter");
                return;
            }
            
        #endif
            
            all.Add(dependency);
        }
    }
}