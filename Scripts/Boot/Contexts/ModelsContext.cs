using System.Collections.Generic;
using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.Loop.Extensions;

namespace TinyMVC.Boot.Contexts {
    public abstract class ModelsContext : IResolving {
        internal readonly List<IBinder> binders;
        internal readonly List<IDependency> dependenciesBinded;
        internal readonly List<IDependency> dependencies;
        
        public sealed class EmptyContext : ModelsContext {
            internal EmptyContext() { }
            
            protected override void Bind() { }
            
            protected override void Create(List<IDependency> _) { }
        }
        
        protected ModelsContext() {
            binders = new List<IBinder>();
            dependenciesBinded = new List<IDependency>();
            dependencies = new List<IDependency>();
        }
        
        public static EmptyContext Empty() => new EmptyContext();
        
        internal void CreateBinders() => Bind();
        
        internal void ApplyBindDependencies(string contextKey) {
            for (int bindId = 0; bindId < binders.Count; bindId++) {
                IBinder binder = binders[bindId];
                
                if (binder is IBindConditions conditions && conditions.IsNeedBinding() == false) {
                    continue;
                }
                
                IDependency dependency = binder.GetDependency();
                ProjectContext.data.Add(contextKey, dependency);
                dependenciesBinded.Add(dependency);
            }
        }
        
        internal void Create() => Create(dependencies);
        
        internal void Unload() {
            dependenciesBinded.TryUnload();
            dependencies.TryUnload();
        }
        
        protected void Add<T>(T binder) where T : Binder => binders.Add(binder);
        
        protected abstract void Bind();
        
        protected virtual void Create(List<IDependency> models) { }
    }
}