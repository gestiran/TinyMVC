using System;
using TinyMVC.Dependencies;
using TinyMVC.Loop;

namespace TinyMVC.Boot.Binding {
    /// <summary> Dependency objects factory </summary>
    /// <typeparam name="T"> Dependency object type </typeparam>
    public abstract class DependencyPoolBinder<T> : Binder where T : IDependency, new() {
        protected abstract int _count { get; }
        
        /// <summary> Internal create first state dependency object </summary>
        /// <returns> Dependency object result created on <see cref="Bind"/> function </returns>
        public override IDependency GetDependency() => Bind();
        
        internal override Type GetBindType() => typeof(DependencyPool<T>);
        
        public DependencyPool<T> Bind() {
            if (this is IInit init) {
                init.Init();
            }
            
            DependencyPool<T> models = new DependencyPool<T>(_count);
            
            for (int modelId = 0; modelId < models.length; modelId++) {
                T model = new T();
                Bind(model, modelId);
                models[modelId] = model;
            }
            
            return models;
        }
        
        protected abstract void Bind(T model, int index);
    }
}