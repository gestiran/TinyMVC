using System;
using System.Collections.Generic;
using TinyMVC.Boot.Binding;
using TinyMVC.Boot.Empty;
using TinyMVC.Dependencies;
using TinyMVC.Loop.Extensions;
using TinyMVC.Debugging;

namespace TinyMVC.Boot.Contexts {
    /// <summary> Contains models initialization </summary>
    public abstract class ModelsContext : IResolving {
        private readonly List<IBinder> _binders;
        private readonly List<IDependency> _models;
        
        protected ModelsContext() {
            _binders = new List<IBinder>();
            _models = new List<IDependency>();
        }
        
        public static ModelsEmptyContext Empty() => new ModelsEmptyContext();
        
        internal void CreateBinders() => Bind();
        
        internal List<IResolving> CreateResolving() {
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
                _models.Add(DebugUtility.CheckAndLogExceptionResult(_binders[bindId].GetDependency));
            }
        }
        
        internal void Create() => Create(_models);
        
        internal void AddDependencies(List<IDependency> dependencies) => dependencies.AddRange(_models);
        
        internal void Unload() => _models.TryUnload();
        
        protected void Add<T>() where T : Binder, new() => _binders.Add(new T());
        
        protected void Add<T>(params Type[] types) where T : Binder, new() => _binders.Add(new BinderLink(new T(), types));
        
        protected void Add<T>(T binder) where T : Binder => _binders.Add(binder);
        
        protected void Add<T>(T binder, params Type[] types) where T : Binder => _binders.Add(binder.AsLink(types));
        
        /// <summary> Create and execute binders, created models will be added by the time Create is called </summary>
        protected abstract void Bind();
        
        /// <summary> Create models and connect initialization </summary>
        /// <param name="models"> Data containers </param>
        protected abstract void Create(List<IDependency> models);
    }
}