using TinyMVC.Dependencies;

namespace TinyMVC.Boot {
    /// <summary> Dependency objects factory </summary>
    /// <typeparam name="T"> Dependency object type </typeparam>
    public abstract class ObservedDependencyListBinder<T> : Binder where T : IDependency, new() {
        private readonly int _capacity;

        protected ObservedDependencyListBinder(int capacity = 4) => _capacity = capacity;

        /// <summary> Internal create first state dependency object </summary>
        /// <returns> Dependency object result created on <see cref="Bind"/> function </returns>
        internal override IDependency GetDependency() {
            ObservedDependencyList<T> model = new ObservedDependencyList<T>(_capacity);
            Bind(model);
            return model;
        }

        /// <summary> Create and load first state dependency object </summary>
        /// <returns> Dependency object result </returns>
        protected abstract void Bind(ObservedDependencyList<T> model);
    }
}