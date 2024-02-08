using TinyMVC.Dependencies;

namespace TinyMVC.Boot {
    /// <summary> Dependency objects factory </summary>
    /// <typeparam name="T"> Dependency object type </typeparam>
    public abstract class DependencyPoolBinder<T> : Binder where T : IDependency, new() {
        private readonly int _count;

        protected DependencyPoolBinder(int count) => _count = count;

        /// <summary> Internal create first state dependency object </summary>
        /// <returns> Dependency object result created on <see cref="Bind"/> function </returns>
        internal override IDependency GetDependency() => Bind();

        public DependencyPool<T> Bind() {
            DependencyPool<T> models = new DependencyPool<T>(_count);

            for (int modelId = 0; modelId < models.length; modelId++) {
                T model = new T();
                Bind(model, modelId);
                models[modelId] = model;
            }

            return models;
        } 
        
        protected abstract void Bind(T model, int index);
    }
}