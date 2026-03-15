// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using TinyMVC.Dependencies;
using TinyReactive;

namespace TinyMVC.Boot.Binding {
    /// <summary> The (internal API) factory responsible for initializing and loading models. </summary>
    public abstract class Binder : IBinder {
        public abstract IDependency GetDependency();
        
        protected string _key { get; private set; }
        
        /// <summary> Reference to the unload method, by default will be called when the current scene is unloaded. </summary>
        protected UnloadPool _unload { get; private set; }
        
        /// <summary> Unique model identifier required for save-load. </summary>
        internal string keyValue {
            get => _key;
            set => _key = value;
        }
        
        /// <summary> Self-reference. </summary>
        public Binder current => this;
        
        /// <summary> Protected/Internal constructor. </summary>
        /// <param name="key"> Unique model identifier required for save-load. </param>
        protected Binder(string key = null) => _key = key;
        
        /// <summary> Creates and initializes the model. </summary>
        /// <returns> The model is ready for work. </returns>
        public abstract IDependency GetDependency();
        
        /// <summary> Get the type of the model being created. </summary>
        /// <returns> Type of model being created. </returns>
        internal abstract Type GetBindType();
        
        /// <summary>
        /// Connect the unload reference.<br/>
        /// Initialization in <see cref="TinyMVC.Boot.SceneContext{T}">SceneContext</see> auto-connect <see cref="TinyReactive.UnloadPool">Unload</see> to the current scene.
        /// </summary>
        /// <param name="unload"> Unload reference. </param>
        internal void ConnectUnload(UnloadPool unload) => _unload = unload;
    }
    
    /// <summary> A typed generic factory responsible for initializing and loading models. </summary>
    /// <typeparam name="T"> Get the type of the model being created. </typeparam>
    public abstract class Binder<T> : Binder where T : IDependency, new() {
        /// <summary> Protected/Internal constructor. </summary>
        /// <param name="key"> Unique model identifier required for save-load. </param>
        protected Binder(string key = null) => keyValue = key;
        
        /// <summary> Recursive call fix. </summary>
        private bool _isCreated;
        
        /// <summary> Current created model </summary>
        private T _model;
        
        /// <summary> Creates and initializes the model. </summary>
        /// <returns> The model is ready for work. </returns>
        public override IDependency GetDependency() {
            T model = new T();
            BindInternal(model);
            Bind(model);
            return model;
        }
        
        internal override Type GetBindType() => typeof(T);
        
        /// <summary> Creates and initializes the model. </summary>
        /// <returns> The model is ready for work. </returns>
        public T Bind() => (T)GetDependency();
        
        /// <summary> Initializes the model parameters. </summary>
        /// <param name="model"> Model reference after <see cref="TinyMVC.Boot.Binding.Binder{T}.BindInternal">internal bind</see> process. </param>
        protected abstract void Bind(T model);
        
        /// <summary>
        /// Internal model initialization.<br/>
        /// Used in <see cref="TinyMVC.Boot.Binding.ActorBinder{T}">ActorBinder</see> for automatic <see cref="TinyMVC.Views.View">View</see> injection.
        /// </summary>
        /// <param name="model"> Empty model. </param>
        internal virtual void BindInternal(T model) { }
    }
}