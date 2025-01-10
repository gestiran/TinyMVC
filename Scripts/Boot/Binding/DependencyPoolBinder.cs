using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    public abstract class DependencyPoolBinder<T> : DependencyAbstractPoolBinder<T> where T : IDependency, new() {
        protected override T New(int index) => new T();
    }
}