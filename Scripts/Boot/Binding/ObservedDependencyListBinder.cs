﻿using System;
using TinyMVC.Dependencies;
using TinyMVC.Loop;

namespace TinyMVC.Boot.Binding {
    /// <summary> Dependency objects factory </summary>
    /// <typeparam name="T"> Dependency object type </typeparam>
    public abstract class ObservedDependencyListBinder<T> : Binder where T : IDependency, new() {
        protected virtual int _capacity { get; } = 4;
        
        /// <summary> Internal create first state dependency object </summary>
        /// <returns> Dependency object result created on <see cref="Bind"/> function </returns>
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
                T model = new T();
                bind(model, i);
                models.Add(model);
            }
        }
        
        /// <summary> Create and load first state dependency object </summary>
        /// <returns> Dependency object result </returns>
        protected abstract void Bind(ObservedDependencyList<T> model);
    }
}