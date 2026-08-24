// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using TinyMVC.Dependencies;
using TinyMVC.Loop;

namespace TinyMVC.Boot.Binding {
    public abstract class ObservedDependencyAbstractListBinder<T> : Binder where T : IDependency {
        protected virtual int _capacity { get; } = 4;
        
        protected ObservedDependencyAbstractListBinder(string key = null) => keyValue = key;
        
        public override IDependency GetDependency() => Bind();
        
        internal override Type GetBindType() => typeof(ObservedDependencyList<T>);
        
        public ObservedDependencyList<T> Bind() {
            if (this is IInit init) {
                init.Init();
            }
            
            ObservedDependencyList<T> model = new ObservedDependencyList<T>(_capacity);
            Bind(model);
            
            return model;
        }
        
        protected void AddCount(ObservedDependencyList<T> models, int count, Action<T, int> bind) {
            for (int i = 0; i < count; i++) {
                T model = New(i);
                bind(model, i);
                models.Add(model);
            }
        }
        
        protected abstract void Bind(ObservedDependencyList<T> model);
        
        protected virtual T New(int index) => default;
    }
}