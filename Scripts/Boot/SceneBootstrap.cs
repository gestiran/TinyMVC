using System.Collections.Generic;
using Sirenix.OdinInspector;
using TinyDI.Dependencies.Models;
using TinyDI.Dependencies.Parameters;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TinyMVC.Boot {
    [DisallowMultipleComponent]
    public abstract class SceneBootstrap<TViews> : MonoBehaviour, IContext where TViews : BootViews {
        [field: SerializeField, BoxGroup("Views")]
        public TViews views { get; private set; }
        public BootControllers controllers { get; protected set; }
        public BootModels models { get; protected set; }
        
        protected BootResources _resources;
        
        public void Create() {
            controllers = CreateControllers();
            models = CreateModels();
            _resources = CreateResources();
            views.Instantiate();
            
            controllers.Create();
            views.Create();
        }

        protected abstract BootControllers CreateControllers();

        protected abstract BootModels CreateModels();

        protected abstract BootResources CreateResources();

        public void Init(ProjectBootstrap context, Scene current) {
            views.Init();
            
            ResolveParameters(context, current);
            ResolveModels(context, current);
            
            controllers.Start();
            views.StartView();
            
            controllers.StartUpdateLoop();
            views.StartUpdateLoop();
        }

        public void Unload() {
            controllers.Dispose();
            views.Dispose();
        }
        
        private void ResolveParameters(ProjectBootstrap context, Scene current) {
            if (_resources is IParametersResolving globalResolving) {
                context.ResolveParameters(globalResolving);
            }
            
            _resources.Create();
            
            ParametersContainer parametersContainer = _resources.CreateContainer();
            
            List<IParametersResolving> parametersResolving = new List<IParametersResolving>();

            controllers.GetParametersResolvers(parametersResolving);
            views.GetParametersResolvers(parametersResolving);

            if (models is IParametersResolving modelsResolving) {
                parametersResolving.Add(modelsResolving);
            }

            context.AddParameters(current, parametersContainer);
            context.ResolveParameters(parametersResolving);
        }

        private void ResolveModels(ProjectBootstrap context, Scene current) {
            if (_resources is IModelsResolving globalResolving) {
                context.ResolveModels(globalResolving);
            }
            
            models.Create();
            
            ModelsContainer modelsContainer = models.CreateContainer();

            List<IModelsResolving> modelsResolving = new List<IModelsResolving>();

            controllers.GetModelsResolvers(modelsResolving);
            views.GetModelsResolvers(modelsResolving);
            
            context.AddModels(current, modelsContainer);
            context.ResolveModels(modelsResolving);
        }
    }
}