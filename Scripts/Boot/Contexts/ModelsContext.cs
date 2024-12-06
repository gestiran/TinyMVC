using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.Loop.Extensions;

namespace TinyMVC.Boot.Contexts {
    public abstract class ModelsContext : IResolving {
        private readonly List<IBinder> _binders;
        private readonly List<IDependency> _models;
        
        public sealed class EmptyContext : ModelsContext {
            internal EmptyContext() { }
            
            protected override void Bind() { }
            
            protected override void Create(List<IDependency> models) { }
        }
        
        protected ModelsContext() {
            _binders = new List<IBinder>();
            _models = new List<IDependency>();
        }
        
        public static EmptyContext Empty() => new EmptyContext();
        
        internal void CreateBinders() => Bind();
        
        internal List<IResolving> GetBindResolving() {
            List<IResolving> resolving = new List<IResolving>(_binders.Count);
            
            for (int binderId = 0; binderId < _binders.Count; binderId++) {
                if (_binders[binderId].current is IResolving bindResolving) {
                    resolving.Add(bindResolving);
                }
            }
            
            return resolving;
        }
        
        internal void ApplyBindDependencies() {
            for (int bindId = 0; bindId < _binders.Count; bindId++) {
                IBinder binder = _binders[bindId];
                
                if (binder is IBindConditions conditions && conditions.IsNeedBinding() == false) {
                    continue;
                }
                
                _models.Add(binder.GetDependency());
            }
        }
        
        internal void Create() => Create(_models);
        
        internal void AddDependencies(List<IDependency> dependencies) => dependencies.AddRange(_models);
        
        internal void Unload() => _models.TryUnload();
        
        protected void Add<T>(T binder, params Type[] types) where T : Binder => _binders.Add(new BinderLink(binder, types));
        
        protected void Add<T>(T binder) where T : Binder => _binders.Add(binder);
        
        protected void AddRuntime<T>(T binder) where T : Binder => ProjectBinding.Add(binder);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void Bind();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void Create(List<IDependency> models) { }
    }
}