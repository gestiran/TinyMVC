using System;
using System.Runtime.CompilerServices;
using TinyMVC.Dependencies;
using TinyMVC.Loop;

namespace TinyMVC.Boot.Binding {
    public abstract class DependencyAbstractPoolBinder<T> : Binder where T : IDependency {
        protected abstract int _count { get; }
        
        protected DependencyAbstractPoolBinder(string key = null) => keyValue = key;
        
        public override IDependency GetDependency() => Bind();
        
        internal override Type GetBindType() => typeof(DependencyPool<T>);
        
        public DependencyPool<T> Bind() {
            if (this is IInit init) {
                init.Init();
            }
            
            DependencyPool<T> models = new DependencyPool<T>(_count);
            FillModels(models);
            Bind(models);
            return models;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void FillModels(DependencyPool<T> models) {
            for (int modelId = 0; modelId < models.length; modelId++) {
                models[modelId] = New(modelId);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void Bind(DependencyPool<T> models) {
            for (int modelId = 0; modelId < models.length; modelId++) {
                Bind(models[modelId], modelId);
            }
        }
        
        protected virtual void Bind(T model, int index) { }
        
        protected abstract T New(int index);
    }
}