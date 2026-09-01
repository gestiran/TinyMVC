// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using TinyMVC.Dependencies;
using TinyReactive;
using TinyUtilities.Logger;

namespace TinyMVC.Boot.Binding {
    /// <summary> The factory responsible for initializing and loading models. </summary>
    public abstract class Binder {
        /// <summary> Unique model identifier required for save-load. </summary>
        protected string _key { get; private set; }
        
        /// <summary> Reference to the unload method, by default will be called when the current scene is unloaded. </summary>
        protected UnloadPool _unload { get; private set; }
        
        /// <summary> Unique model identifier required for save-load. </summary>
        internal string keyValue {
            get => _key;
            set => _key = value;
        }
        
        /// <summary> Override default key value. </summary>
        /// <param name="key"> Unique model identifier required for save-load. </param>
        protected Binder(string key = null) => _key = key;
        
        /// <summary> Creates and initializes the model. </summary>
        /// <returns> The model is ready for work. </returns>
        internal abstract IDependency GetDependency();
        
        /// <summary> Get the type of the model being created. </summary>
        /// <returns> Type of model being created. </returns>
        internal abstract Type GetBindType();
        
        /// <summary> Connect the unload reference. </summary>
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
        internal override IDependency GetDependency() {
            if (_isCreated) {
                DebugUtility.LogError($"{GetType().Name} - Self created!");
                return _model;
            }
            
            _isCreated = true;
            
            _model = new T();
            BindInternal(_model);
            Bind(_model);
            return _model;
        }
        
        /// <summary> Get the type of the model being created. </summary>
        /// <returns> Type of model being created. </returns>
        internal override Type GetBindType() => typeof(T);
        
        /// <summary> Creates and initializes the model. </summary>
        /// <returns> The model is ready for work. </returns>
        public T Bind() => (T)GetDependency();
        
        /// <summary> Resets the previous creation marker and bind next model. </summary>
        /// <returns> The new model is ready for work. </returns>
        public T ReBind() {
            Reset();
            return Bind();
        }
        
        /// <summary> Resets the previous creation marker. </summary>
        public void Reset() => _isCreated = false;
        
        /// <summary> Initializes the model parameters. </summary>
        /// <param name="model"> Model reference after <see cref="TinyMVC.Boot.Binding.Binder{T}.BindInternal">internal bind</see> process. </param>
        protected abstract void Bind(T model);
        
        /// <summary> Internal model initialization. </summary>
        /// <param name="model"> Empty model. </param>
        internal virtual void BindInternal(T model) { }
    }
}