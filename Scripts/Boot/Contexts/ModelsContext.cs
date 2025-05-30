using System.Collections.Generic;
using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.Loop.Extensions;

namespace TinyMVC.Boot.Contexts {
    public abstract class ModelsContext : IResolving {
        internal readonly List<IDependency> dependenciesBinded;
        internal readonly List<IDependency> dependencies;
        internal string key;
        
        public sealed class EmptyContext : ModelsContext {
            internal EmptyContext() { }
            
            protected override void Bind() { }
            
            protected override void Create(List<IDependency> _) { }
        }
        
        protected ModelsContext() {
            dependenciesBinded = new List<IDependency>();
            dependencies = new List<IDependency>();
        }
        
        public static EmptyContext Empty() => new EmptyContext();
        
        internal void CreateBinders(string contextKey) {
            key = contextKey;
            Bind();
        }
        
        internal void Create() => Create(dependencies);
        
        internal void Unload() {
            dependenciesBinded.TryUnload();
            dependencies.TryUnload();
        }
        
        protected void Add<T>(T binder) where T : Binder {
            if (binder is IBindConditions conditions && conditions.IsNeedBinding() == false) {
                return;
            }
            
            IDependency dependency = binder.GetDependency();
            ProjectContext.data.Add(key, dependency);
            dependenciesBinded.Add(dependency);
        }
        
        protected abstract void Bind();
        
        protected virtual void Create(List<IDependency> models) { }
    }
}