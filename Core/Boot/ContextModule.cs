using System.Collections.Generic;
using TinyMVC.Boot.Binding;
using TinyMVC.Boot.Contexts;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyReactive.Fields;

namespace TinyMVC.Boot {
    public abstract class ContextModule : IContextComponent {
        private List<IController> _systems;
        private List<ActionListener> _initSystemsLazy;
        private ModelsContext _models;
        private List<IDependency> _parameters;
        
        void IContextComponent.Instantiate() { }
        
        void IContextComponent.CreateControllers(List<IController> systems, List<ActionListener> initSystemsLazy) {
            _systems = systems;
            _initSystemsLazy = initSystemsLazy;
            CreateControllers();
        }
        
        internal void CheckAndAdd<T>(List<T> list) {
            for (int systemId = 0; systemId < _systems.Count; systemId++) {
                if (_systems[systemId] is T controller) {
                    list.Add(controller);
                }
            }
        }
        
        void IContextComponent.CreateBinders<T>(T context) {
            _models = context;
            CreateBinders();
        }
        
        void IContextComponent.CreateModels(List<IDependency> models) {
            CreateModels(models);
        }
        
        void IContextComponent.CreateParameters(List<IDependency> parameters) {
            _parameters = parameters;
            CreateParameters();
        }
        
        protected virtual void CreateControllers() {
            // Empty
        }
        
        protected virtual void CreateBinders() {
            // Empty
        }
        
        protected virtual void CreateModels(List<IDependency> models) {
            // Empty
        }
        
        protected virtual void CreateParameters() {
            // Empty
        }
        
        protected void Add<T>() where T : IController, new() => _initSystemsLazy.Add(() => _systems.Add(new T()));
        
        protected void AddBinder<T>(T binder) where T : Binder {
            if (binder is IBindConditions conditions && conditions.IsNeedBinding() == false) {
                return;
            }
            
            IDependency dependency = binder.GetDependency();
            ProjectContext.data.Add(_models.key, dependency);
            _models.dependenciesBinded.Add(dependency);
        }
        
        protected void Load<T>(T dependency) where T : IDependency => _parameters.Add(dependency);
        
        void IContextComponent.AddComponentsViews(IViewsContext context) { }
    }
}