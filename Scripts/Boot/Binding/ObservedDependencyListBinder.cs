using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    /// <summary> Dependency objects factory </summary>
    /// <typeparam name="T"> Dependency object type </typeparam>
    public abstract class ObservedDependencyListBinder<T> : Binder where T : IDependency, new() {
        protected abstract int _capacity { get; }

        /// <summary> Internal create first state dependency object </summary>
        /// <returns> Dependency object result created on <see cref="Bind"/> function </returns>
        internal override IDependency GetDependency() => Bind();

        public ObservedDependencyList<T> Bind() {
            ObservedDependencyList<T> model = new ObservedDependencyList<T>(_capacity);
            Bind(model);
            return model;
        }

        /// <summary> Create and load first state dependency object </summary>
        /// <returns> Dependency object result </returns>
        protected abstract void Bind(ObservedDependencyList<T> model);
    }
}