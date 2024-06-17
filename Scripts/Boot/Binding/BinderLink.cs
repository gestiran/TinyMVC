using System;
using JetBrains.Annotations;
using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    internal sealed class BinderLink : IBinder {
        public Binder current => _binder;
        
        private readonly Binder _binder;
        private readonly Type[] _types;
        
        public BinderLink(Binder binder) {
            _binder = binder;
            
            _types = new[] {
                binder.GetBindType()
            };
        }
        
        public BinderLink(Binder binder, [NotNull] params Type[] types) {
            this._binder = binder;
            this._types = types;
        }
        
        public IDependency GetDependency() => new Dependency(_binder.GetDependency(), _types);
    }
}