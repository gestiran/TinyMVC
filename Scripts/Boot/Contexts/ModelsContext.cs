using System.Collections.Generic;
using TinyMVC.Boot.Binding;
using TinyMVC.Boot.Empty;
using TinyMVC.Dependencies;
using TinyMVC.Extensions;

#if UNITY_EDITOR
using System;
using UnityEngine;
#endif

namespace TinyMVC.Boot.Contexts {
    /// <summary> Contains models initialization </summary>
    public abstract class ModelsContext : IResolving {
        private readonly List<Binder> _binders;
        private readonly List<IDependency> _models;

        protected ModelsContext() {
            _binders = new List<Binder>();
            _models = new List<IDependency>();
        }
        
        public static ModelsEmptyContext Empty() => new ModelsEmptyContext();

        internal void CreateBinders() => Bind();

        internal List<IResolving> CreateResolving() {
            List<IResolving> resolving = new List<IResolving>(_binders.Count);
            resolving.AddRange(_binders);
            return resolving;
        }

        internal void ApplyBindDependencies() {
            for (int bindId = 0; bindId < _binders.Count; bindId++) {
            #if UNITY_EDITOR
                try {
                #endif

                    _models.Add(_binders[bindId].GetDependency());
                    
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"BindError: {_binders[bindId].GetType().Name}\n{e}");
                }
            #endif
            }
        }

        internal void Create() => Create(_models);

        internal void AddDependencies(List<IDependency> dependencies) => dependencies.AddRange(_models);

        internal void Unload() => _models.TryUnload();

        protected void Add<T>() where T : Binder, new() => _binders.Add(new T());
        
        protected void Add<T>(T binder) where T : Binder => _binders.Add(binder);

        /// <summary> Create and execute binders, created models will be added by the time Create is called </summary>
        protected abstract void Bind();
        
        /// <summary> Create models and connect initialization </summary>
        /// <param name="models"> Data containers </param>
        protected abstract void Create(List<IDependency> models);
    }
}