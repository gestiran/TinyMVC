using System;
using JetBrains.Annotations;
using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    /// <summary> Dependency objects factory </summary>
    public abstract class Binder : IBinder, IResolving {
        /// <summary> Internal create first state dependency object </summary>
        /// <returns> Dependency object result created on <see cref="Binder{T}.Bind(T)"/> function </returns>
        public abstract IDependency GetDependency();
        
        public Binder current => this;
        
        internal abstract Type GetBindType();
        
        protected T Add<T>() where T : Binder, new() => new T();
        
        protected T Add<T>([NotNull] IDependency dependency) where T : Binder, new() {
            T binder = new T();
            ResolveUtility.Resolve(binder, new DependencyContainer(dependency));
            
            return binder;
        }
        
        protected T Add<T>([NotNull] params IDependency[] dependencies) where T : Binder, new() {
            T binder = new T();
            ResolveUtility.Resolve(binder, new DependencyContainer(dependencies));
            
            return binder;
        }
        
        protected T Add<T>(T binder, [NotNull] IDependency dependency) where T : Binder {
            ResolveUtility.Resolve(binder, new DependencyContainer(dependency));
            
            return binder;
        }
        
        protected T Add<T>(T binder, [NotNull] params IDependency[] dependencies) where T : Binder {
            ResolveUtility.Resolve(binder, new DependencyContainer(dependencies));
            
            return binder;
        }
    }
    
    /// <summary> Dependency objects factory </summary>
    /// <typeparam name="T"> Dependency object type </typeparam>
    public abstract class Binder<T> : Binder where T : class, IDependency, new() {
        /// <summary> Internal create first state dependency object </summary>
        /// <returns> Dependency object result created on <see cref="Bind"/> function </returns>
        public override IDependency GetDependency() {
            T model = new T();
            Bind(model);
            
            return model;
        }
        
        internal override Type GetBindType() => typeof(T);
        
        public T Bind() => GetDependency() as T;
        
        /// <summary> Create and load first state dependency object </summary>
        /// <returns> Dependency object result </returns>
        protected abstract void Bind(T model);
    }
}