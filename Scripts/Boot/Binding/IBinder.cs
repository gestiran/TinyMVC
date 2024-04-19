using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    internal interface IBinder {
        public IDependency GetDependency();
        public Binder binder { get; }
    }
}