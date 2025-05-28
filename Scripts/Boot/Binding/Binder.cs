using System;
using JetBrains.Annotations;
using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    public abstract class Binder : IBinder, IApplyResolving {
        public abstract IDependency GetDependency();
        
        protected string _key { get; private set; }
        
        internal string keyValue {
            get => _key;
            set => _key = value;
        }
        
        public Binder current => this;
        
        protected Binder(string key = null) => _key = key;
        
        public virtual void ApplyResolving() { }
        
        internal abstract Type GetBindType();
        
        protected bool TryBind<T>(out T model) where T : IDependency, new() => ProjectBinding.TryBind(out model);
        
        protected bool TryBind<T>(out T model, params IDependency[] dependencies) where T : IDependency, new() => ProjectBinding.TryBind(out model, dependencies);
        
        protected T Add<T>(string key = null) where T : Binder, new() {
            T binder = new T();
            binder._key = key;
            ResolveUtility.Resolve(binder);
            return binder;
        }
        
        protected T Add<T>([NotNull] IDependency dependency) where T : Binder, new() {
            T binder = new T();
            ResolveUtility.Resolve(binder, new DependencyContainer(dependency));
            return binder;
        }
        
        protected T Add<T>(string key, [NotNull] IDependency dependency) where T : Binder, new() {
            T binder = new T();
            binder._key = key;
            ResolveUtility.Resolve(binder, new DependencyContainer(dependency));
            return binder;
        }
        
        protected T Add<T>([NotNull] params IDependency[] dependencies) where T : Binder, new() {
            T binder = new T();
            ResolveUtility.Resolve(binder, new DependencyContainer(dependencies));
            return binder;
        }
        
        protected T Add<T>(string key, [NotNull] params IDependency[] dependencies) where T : Binder, new() {
            T binder = new T();
            binder._key = key;
            ResolveUtility.Resolve(binder, new DependencyContainer(dependencies));
            return binder;
        }
        
        protected T Add<T>(T binder, [NotNull] IDependency dependency) where T : Binder {
            ResolveUtility.Resolve(binder, new DependencyContainer(dependency));
            return binder;
        }
        
        protected T Add<T>(T binder, string key, [NotNull] IDependency dependency) where T : Binder {
            binder._key = key;
            ResolveUtility.Resolve(binder, new DependencyContainer(dependency));
            return binder;
        }
        
        protected T Add<T>(T binder, [NotNull] params IDependency[] dependencies) where T : Binder {
            ResolveUtility.Resolve(binder, new DependencyContainer(dependencies));
            return binder;
        }
        
        protected T Add<T>(T binder, string key, [NotNull] params IDependency[] dependencies) where T : Binder {
            binder._key = key;
            ResolveUtility.Resolve(binder, new DependencyContainer(dependencies));
            return binder;
        }
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