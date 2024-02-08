﻿using TinyMVC.Dependencies;

namespace TinyMVC.Boot {
    /// <summary> Dependency objects factory </summary>
    public abstract class Binder {
        /// <summary> Internal create first state dependency object </summary>
        /// <returns> Dependency object result created on <see cref="Binder{T}.Bind()"/> function </returns>
        internal abstract IDependency GetDependency();
    }
    
    /// <summary> Dependency objects factory </summary>
    /// <typeparam name="T"> Dependency object type </typeparam>
    public abstract class Binder<T> : Binder where T : IDependency, new() {
        /// <summary> Internal create first state dependency object </summary>
        /// <returns> Dependency object result created on <see cref="Bind"/> function </returns>
        internal override IDependency GetDependency() {
            T model = new T();
            Bind(model);
            return model;
        }

        /// <summary> Create and load first state dependency object </summary>
        /// <returns> Dependency object result </returns>
        protected abstract void Bind(T model);
    }
}