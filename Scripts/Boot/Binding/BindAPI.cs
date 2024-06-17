using System;
using System.Collections.Generic;
using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    public sealed class BindAPI {
        private readonly Dictionary<Type, Binder> _binders;
        
        private const int _CAPACITY = 64;
        
        internal BindAPI() => _binders = new Dictionary<Type, Binder>(_CAPACITY);
        
        public T Bind<T>() where T : class, IDependency, new() {
            if (_binders.TryGetValue(typeof(T), out Binder data)) {
                // TODO : DI
                return (T)data.GetDependency();
            }
            
            return new T();
        }
        
        public void Add<T>(Binder binder) where T : IDependency => _binders.Add(typeof(T), binder);
        
        public void Remove<T>() where T : IDependency => _binders.Remove(typeof(T));
    }
}