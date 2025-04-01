using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.Loop.Extensions;

namespace TinyMVC.Boot.Contexts {
    public abstract class ModelsContext : IResolving {
        internal DependencyContainer initContainer;
        internal readonly List<IBinder> binders;
        internal readonly List<IDependency> dependenciesBinded;
        internal readonly List<IDependency> dependencies;
        
        public sealed class EmptyContext : ModelsContext {
            internal EmptyContext() { }
            
            protected override void Bind() { }
            
            protected override void Create(List<IDependency> _) { }
        }
        
        protected ModelsContext() {
            initContainer = DependencyContainer.empty;
            binders = new List<IBinder>();
            dependenciesBinded = new List<IDependency>();
            dependencies = new List<IDependency>();
        }
        
        public static EmptyContext Empty() => new EmptyContext();
        
        internal void CreateBinders() => Bind();
        
        internal List<IResolving> GetBindResolving() {
            List<IResolving> resolving = new List<IResolving>(binders.Count);
            
            for (int binderId = 0; binderId < binders.Count; binderId++) {
                if (binders[binderId].current is IResolving bindResolving) {
                    resolving.Add(bindResolving);
                }
            }
            
            return resolving;
        }
        
        internal void ApplyBindDependencies(string contextKey) {
            for (int bindId = 0; bindId < binders.Count; bindId++) {
                IBinder binder = binders[bindId];
                
                if (binder is IBindConditions conditions && conditions.IsNeedBinding() == false) {
                    continue;
                }
                
                if (binder is IApplyResolving applyResolving) {
                    applyResolving.ApplyResolving();
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
        
        protected void Add<T>(T binder, params Type[] types) where T : Binder => binders.Add(new BinderLink(binder, types));
        
        protected void Add<T>(T binder) where T : Binder => binders.Add(binder);
        
        protected void AddRuntime<T>(T binder) where T : Binder => ProjectBinding.Add(binder);
        
        protected T Resolve<T>(T binder) where T : Binder {
            ResolveUtility.Resolve(binder, initContainer);
            ResolveUtility.TryApply(binder);
            return binder;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void Bind();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void Create(List<IDependency> models) { }
    }
}