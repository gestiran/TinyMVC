using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Contexts;
using TinyMVC.Boot.Empty;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Views;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TinyMVC.Debugging.Exceptions;
#endif

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
    /// <summary> Scene initialization order </summary>
    public abstract class SceneContext : SceneContext<ViewsEmptyContext> { }

    /// <summary> Scene initialization order </summary>
    /// <typeparam name="TViews"> Serialized views class to store references to scene objects </typeparam>
    public abstract class SceneContext<TViews> : MonoBehaviour, IContext where TViews : ViewsContext {
        [field: SerializeField
            #if ODIN_INSPECTOR && UNITY_EDITOR
                , BoxGroup("Views")
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

            _controllers.CreateControllers();
            views.CreateViews();

            if (this is IGlobalContext) {
                views.ApplyDontDestroyOnLoad();
            }
        }

        async Task IContext.InitAsync(ProjectContext context, int sceneId) {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            try {
            #endif
                PreInitResolve(context);
                
                await _controllers.InitAsync(CreateControllerConnector(context, sceneId));
                await views.InitAsync(CreateViewConnector(context, sceneId));

                Resolve(context, sceneId);

                await _controllers.BeginPlay();
                await views.BeginPlay();

                List<IFixedTick> fixedTicks = new List<IFixedTick>();
                List<ITick> ticks = new List<ITick>();
                List<ILateTick> lateTicks = new List<ILateTick>();

                _controllers.CheckAndAdd(fixedTicks);
                _controllers.CheckAndAdd(ticks);
                _controllers.CheckAndAdd(lateTicks);

                views.CheckAndAdd(fixedTicks);
                views.CheckAndAdd(ticks);
                views.CheckAndAdd(lateTicks);

                context.AddFixedTicks(sceneId, fixedTicks);
                context.AddTicks(sceneId, ticks);
                context.AddLateTicks(sceneId, lateTicks);

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            } catch (Exception exception) {
                if (sceneId < 0) {
                    Debug.LogException(new SceneException($"(ID:{sceneId})", exception), this);
                } else {
                    Debug.LogException(new SceneException(SceneManager.GetSceneByBuildIndex(sceneId).name, exception), this);
                }
            }
        #endif
        }

        void IContext.Unload() {
            _controllers.Unload();
            views.Unload();
            _models.Unload();
        }

        protected abstract ControllersContext CreateControllers();

        protected abstract ModelsContext CreateModels();

        protected abstract ParametersContext CreateParameters();

        private void PreInitResolve(ProjectContext context) {
            List<IResolving> resolvers = new List<IResolving>();

            _controllers.CheckAndAdd(resolvers);
            views.CheckAndAdd(resolvers);

            context.ResolveWithoutApply(resolvers);
        }
        
        private void Resolve(ProjectContext context, int sceneId) {
            if (_parameters is IResolving parametersResolving) {
                context.Resolve(parametersResolving);
            }

            List<IDependency> dependencies = new List<IDependency>();
            List<IDependency> bindDependencies = new List<IDependency>();

            _parameters.Create();

            _parameters.AddDependencies(dependencies);
            _parameters.AddDependencies(bindDependencies);

            context.ResolveWithoutApply(_models, dependencies);

            _models.CreateBinders();

            views.GetDependencies(bindDependencies);

            context.ResolveWithoutApply(_models.CreateResolving(), bindDependencies);

            bindDependencies.Clear();
            _models.ApplyBindDependencies();
            _models.AddDependencies(bindDependencies);

            ResolveUtility.Resolve(_models, this, new DependencyContainer(bindDependencies));

            _models.Create();
            
            _models.AddDependencies(dependencies);

            context.AddContainer(sceneId, new DependencyContainer(dependencies));

            List<IResolving> resolvers = new List<IResolving>();

            _controllers.CheckAndAdd(resolvers);
            views.CheckAndAdd(resolvers);

            context.Resolve(resolvers);
        }

        private Controller.Connector CreateControllerConnector(ProjectContext context, int sceneId) {
            Controller.Connector connector = new Controller.Connector();

            connector.connect = controller => Connect(controller, context, sceneId);
            connector.connectWithoutDependencies = (controller, resolve) => Connect(controller, resolve, context, sceneId);
            connector.connectArray = controllers => Connect(controllers, context, sceneId);
            connector.disconnect = controller => Disconnect(controller, context, sceneId);
            connector.disconnectArray = controllers => Disconnect(controllers, context, sceneId);

            return connector;
        }

        private View.Connector CreateViewConnector(ProjectContext context, int sceneId) {
            View.Connector connector = new View.Connector();

            connector.connect = view => Connect(view, context, sceneId);
            connector.connectWithoutDependencies = (view, resolve) => Connect(view, resolve, context, sceneId);
            connector.connectArray = view => Connect(view, context, sceneId);
            connector.disconnect = view => Disconnect(view, context, sceneId);
            connector.disconnectArray = view => Disconnect(view, context, sceneId);

            return connector;
        }

        private void Connect(IController controller, ProjectContext context, int sceneId) {
            _controllers.InitSubController(controller, context.Resolve, loop => context.ConnectLoop(sceneId, loop));
        }
        
        private void Connect(IController controller, Action resolve, ProjectContext context, int sceneId) {
            _controllers.InitSubController(controller, _ => resolve(), loop => context.ConnectLoop(sceneId, loop));
        }

        private void Connect(IController[] controller, ProjectContext context, int sceneId) {
            _controllers.InitSubController(controller, context.Resolve, loop => context.ConnectLoop(sceneId, loop));
        }

        private void Disconnect(IController controller, ProjectContext context, int sceneId) {
            _controllers.DeInitSubController(controller, loop => context.DisconnectLoop(sceneId, loop));
        }

        private void Disconnect(IController[] controller, ProjectContext context, int sceneId) {
            _controllers.DeInitSubController(controller, loop => context.DisconnectLoop(sceneId, loop));
        }

        private void Connect(IView view, ProjectContext context, int sceneId) {
            views.InitSubView(view, context.Resolve, loop => context.ConnectLoop(sceneId, loop));
        }
        
        private void Connect(IView view, Action resolve, ProjectContext context, int sceneId) {
            views.InitSubView(view, _ => resolve(), loop => context.ConnectLoop(sceneId, loop));
        }

        private void Connect(IView[] view, ProjectContext context, int sceneId) {
            views.InitSubView(view, context.Resolve, loop => context.ConnectLoop(sceneId, loop));
        }

        private void Disconnect(IView view, ProjectContext context, int sceneId) => views.DeInitSubView(view, loop => context.DisconnectLoop(sceneId, loop));

        private void Disconnect(IView[] view, ProjectContext context, int sceneId) => views.DeInitSubView(view, loop => context.DisconnectLoop(sceneId, loop));

    #if UNITY_EDITOR
    #if ODIN_INSPECTOR
        [Button("Generate"), ShowIn(PrefabKind.InstanceInScene)]
    #else
        [ContextMenu("Generate")]
    #endif
        private void Reset() {
            if (views != null) {
                views.Generate_Editor();    
            }
            
            UnityEditor.EditorUtility.SetDirty(gameObject);
        }

    #endif
    }
}