using TinyMVC.Dependencies;

namespace TinyMVC.Boot {
    /// <summary> Dependency objects factory </summary>
    public abstract class Binder {
        internal abstract IDependency GetDependency();
    }
    
    /// <summary> Dependency objects factory </summary>
    /// <typeparam name="T"> Dependency object type </typeparam>
    public abstract class Binder<T> : Binder where T : IDependency {
        internal override IDependency GetDependency() => Bind();

        /// <summary> Create and load first state dependency object </summary>
        /// <returns> Dependency object result </returns>
        public abstract T Bind();
    }
}