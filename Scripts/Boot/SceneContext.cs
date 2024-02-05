using System.Collections.Generic;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using UnityEngine;
using UnityEngine.SceneManagement;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
    /// <summary> Scene initialization order </summary>
    /// <typeparam name="TViews"> Serialized views class to store references to scene objects </typeparam>
    [DisallowMultipleComponent]
    public abstract class SceneContext<TViews> : MonoBehaviour, IContext where TViews : ViewsContext {
        [field: SerializeField
    #if ODIN_INSPECTOR && UNITY_EDITOR
        ,BoxGroup("Views")
    #endif
        ]
        public TViews views { get; private set; }

        private ControllersContext _controllers;
        private ModelsContext _models;
        private ParametersContext _parameters;

        void IContext.Create() {
            _controllers = CreateControllers();
            _models = CreateModels();
            _parameters = CreateParameters();
            
            views.Instantiate();

            _controllers.Create();
            views.Create();

            if (this is IGlobalContext) {
                views.ApplyDontDestroyOnLoad();
            }
        }
        
        void IContext.Init(ProjectContext context, ref Scene current) {
            _controllers.Init();
            views.Init();

            Resolve(context, ref current);

            _controllers.BeginPlay();
            views.BeginPlay();

            List<IFixedTick> fixedTicks = new List<IFixedTick>();
            List<ITick> ticks = new List<ITick>();
            List<ILateTick> lateTicks = new List<ILateTick>();

            _controllers.CheckAndAdd(fixedTicks);
            _controllers.CheckAndAdd(ticks);
            _controllers.CheckAndAdd(lateTicks);
            
            views.CheckAndAdd(fixedTicks);
            views.CheckAndAdd(ticks);
            views.CheckAndAdd(lateTicks);
            
            context.AddFixedTicks(ref current, fixedTicks);
            context.AddTicks(ref current, ticks);
            context.AddLateTicks(ref current, lateTicks);
        }

        void IContext.Unload() {
            _controllers.Unload();
            views.Unload();
            _models.Unload();
        }
        
        private void Resolve(ProjectContext context, ref Scene current) {
            if (_parameters is IResolving parametersResolving) {
                context.Resolve(parametersResolving);
            }
            
            List<IDependency> dependencies = new List<IDependency>();
            
            _parameters.Create();
            _parameters.AddDependencies(dependencies);
            
            if (_models is IResolving modelsResolving) {
                context.ResolveWithoutApply(modelsResolving, new DependencyContainer(dependencies));

                List<IDependency> bindDependencies = new List<IDependency>();
                _models.Bind();
                _models.AddDependencies(bindDependencies);
                
                ResolveUtility.Resolve(modelsResolving, new DependencyContainer(bindDependencies));
            } else {
                _models.Bind();
            }
            
            _models.Create();
            _models.AddDependencies(dependencies);
            
            context.AddContainer(ref current, new DependencyContainer(dependencies));
            
            List<IResolving> resolvers = new List<IResolving>();
            
            _controllers.CheckAndAdd(resolvers);
            views.CheckAndAdd(resolvers);
            
            context.Resolve(resolvers);
        }
        
        protected abstract ControllersContext CreateControllers();

        protected abstract ModelsContext CreateModels();

        protected abstract ParametersContext CreateParameters();
    }
}