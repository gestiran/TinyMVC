using System.Collections.Generic;
using TinyMVC.Boot.Binding;
using TinyMVC.Boot.Empty;
using TinyMVC.Dependencies;
using TinyMVC.Loop.Extensions;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TinyMVC.Exceptions;
using System;
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

        internal void CreateBinders() {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            try {
            #endif

                Bind();

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            } catch (Exception exception) {
                throw new BindException(exception);
            }
        #endif
        }

        internal List<IResolving> CreateResolving() {
            List<IResolving> resolving = new List<IResolving>(_binders.Count);
            resolving.AddRange(_binders);
            return resolving;
        }

        internal void ApplyBindDependencies() {
            for (int bindId = 0; bindId < _binders.Count; bindId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif

                    _models.Add(_binders[bindId].GetDependency());

                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new BindException(exception);
                }
            #endif
            }
        }

        internal void Create() {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            try {
            #endif

                Create(_models);

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            } catch (Exception exception) {
                throw new ModelsException(exception);
            }
        #endif
        }

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