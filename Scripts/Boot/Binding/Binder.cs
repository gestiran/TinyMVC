// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using TinyMVC.Dependencies;
using TinyReactive;

namespace TinyMVC.Boot.Binding {
    public abstract class Binder : IBinder {
        public abstract IDependency GetDependency();
        
        protected string _key { get; private set; }
        protected UnloadPool _unload { get; private set; }
        
        internal string keyValue {
            get => _key;
            set => _key = value;
        }
        
        public Binder current => this;
        
        protected Binder(string key = null) => _key = key;
        
        internal abstract Type GetBindType();
        
        internal void ConnectUnload(UnloadPool unload) => _unload = unload;
    }
    
    public abstract class Binder<T> : Binder where T : IDependency, new() {
        protected Binder(string key = null) => keyValue = key;
        
        public override IDependency GetDependency() {
            T model = new T();
            BindInternal(model);
            Bind(model);
            return model;
        }
        
        internal override Type GetBindType() => typeof(T);
        
        public T Bind() => (T)GetDependency();
        
        protected abstract void Bind(T model);
        
        internal virtual void BindInternal(T model) { }
    }
}