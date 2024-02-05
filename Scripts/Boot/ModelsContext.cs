using System.Collections.Generic;
using TinyMVC.Dependencies;
using TinyMVC.Extensions;

namespace TinyMVC.Boot {
    /// <summary> Contains models initialization </summary>
    public abstract class ModelsContext {
        private readonly List<IDependency> _models;

        protected ModelsContext() => _models = new List<IDependency>();
        
        internal void Bind() {
            List<Binder> binders = new List<Binder>();
            
            Bind(binders);

            foreach (Binder binder in binders) {
                _models.Add(binder.GetDependency());
            }
        }
        
        internal void Create() => Create(_models);

        internal void AddDependencies(List<IDependency> dependencies) => dependencies.AddRange(_models);
        
        internal void Unload() => _models.TryUnload();

        /// <summary> Create and execute binders, created models will be added by the time Create is called </summary>
        /// <param name="binders"> Data Converter, View to Model </param>
        protected abstract void Bind(List<Binder> binders);
        
        /// <summary> Create models and connect initialization </summary>
        /// <param name="models"> Data containers </param>
        protected abstract void Create(List<IDependency> models);
    }
}