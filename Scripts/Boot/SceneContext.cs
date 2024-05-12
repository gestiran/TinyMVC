using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Contexts;
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
    /// <typeparam name="TViews"> Serialized views class to store references to scene objects </typeparam>
    public abstract class SceneContext<TViews> : SceneContext, IContext where TViews : ViewsContext {
        [field: SerializeField
            #if ODIN_INSPECTOR && UNITY_EDITOR
                , BoxGroup("Views")
    #endif
        ]
        public TViews views { get; private set; }

        private ControllersContext _controllers;
        private ModelsContext _models;
        private ParametersContext _parameters;

        async Task IContext.Create() {
            Add(this);

            _controllers = CreateControllers();
            _models = CreateModels();
            _parameters = CreateParameters();

            views.Instantiate();

            await _controllers.CreateControllers();
            views.CreateViews();

            if (this is IGlobalContext) {
                views.ApplyDontDestroyOnLoad();
            }
        }

        async Task IContext.InitAsync(ProjectContext context, int sceneId) {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            try {
            #endif
                PreInitResolve();

                await _controllers.InitAsync(sceneId);
                await views.InitAsync(sceneId);

                Resolve(sceneId);

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
            Remove(this);
        }

        protected abstract ControllersContext CreateControllers();

        protected abstract ModelsContext CreateModels();

        protected abstract ParametersContext CreateParameters();

        private void PreInitResolve() {
            List<IResolving> resolvers = new List<IResolving>();

            _controllers.CheckAndAdd(resolvers);
            views.CheckAndAdd(resolvers);

            ProjectContext.data.ResolveWithoutApply(resolvers);
        }

        private void Resolve(int sceneId) {
            if (_parameters is IResolving parametersResolving) {
                ProjectContext.data.Resolve(parametersResolving);
            }

            List<IDependency> dependencies = new List<IDependency>();
            List<IDependency> bindDependencies = new List<IDependency>();

            _parameters.Create();

            _parameters.AddDependencies(dependencies);
            _parameters.AddDependencies(bindDependencies);

            ProjectContext.data.ResolveWithoutApply(_models);
            ProjectContext.data.ResolveWithoutApply(new DependencyContainer(dependencies), _models);

            _models.CreateBinders();

            views.GetDependencies(bindDependencies);

            List<IResolving> resolvers = _models.CreateResolving();
            
            ProjectContext.data.ResolveWithoutApply(resolvers);
            ProjectContext.data.ResolveWithoutApply(new DependencyContainer(bindDependencies), resolvers);

            bindDependencies.Clear();
            _models.ApplyBindDependencies();
            _models.AddDependencies(bindDependencies);

            ResolveUtility.Resolve(_models, new DependencyContainer(bindDependencies));

            _models.Create();

            _models.AddDependencies(dependencies);

            ProjectContext.data.Add(sceneId, new DependencyContainer(dependencies));
            resolvers.Clear();

            _controllers.CheckAndAdd(resolvers);
            views.CheckAndAdd(resolvers);

            ProjectContext.data.Resolve(resolvers);
        }

        internal override void Connect(IView view, int sceneId, Action<IResolving> resolve) => views.Connect(view, sceneId, resolve);

        internal override void Connect(IController view, int sceneId, Action<IResolving> resolve) => _controllers.Connect(view, sceneId, resolve);

        internal override void Disconnect(IView view, int sceneId) => views.Disconnect(view, sceneId);

        internal override void Disconnect(IController view, int sceneId) => _controllers.Disconnect(view, sceneId);

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

    /// <summary> Scene initialization order </summary>
    public abstract class SceneContext : MonoBehaviour {
        private static readonly Dictionary<int, SceneContext> _contexts;

        private const int _SCENES_COUNT = 16;

        static SceneContext() => _contexts = new Dictionary<int, SceneContext>(_SCENES_COUNT);

        internal static SceneContext GetContext(int buildIndex) => _contexts[buildIndex];

        protected static void Add(SceneContext context) => _contexts.Add(context.gameObject.scene.buildIndex, context);

        protected static void Remove(SceneContext context) => _contexts.Remove(context.gameObject.scene.buildIndex);

        internal abstract void Connect(IView view, int sceneId, Action<IResolving> resolve);

        internal abstract void Connect(IController controller, int sceneId, Action<IResolving> resolve);

        internal abstract void Disconnect(IView view, int sceneId);

        internal abstract void Disconnect(IController controller, int sceneId);

    #if UNITY_EDITOR
    
        [UnityEditor.InitializeOnLoadMethod]
        private static void RevertChanges() => UnityEditor.EditorApplication.playModeStateChanged += PlayModeChange;

        private static void PlayModeChange(UnityEditor.PlayModeStateChange state) {
            if (state != UnityEditor.PlayModeStateChange.ExitingPlayMode) {
                return;
            }
            
            _contexts.Clear();
        }


    #endif
    }
}