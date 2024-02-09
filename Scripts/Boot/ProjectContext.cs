using System;
using System.Collections.Generic;
using TinyMVC.Boot.Contexts;
using TinyMVC.Boot.Helpers;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;
using UnityEngine;
using UnityEngine.SceneManagement;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
    [DisallowMultipleComponent]
    internal sealed class ProjectContext : MonoBehaviour {
        private List<BootstrapContext> _contexts;
        private LoopContext _loopContext;

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [BoxGroup("Debug"), ShowInInspector, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowFoldout = false)]
    #endif
        private List<DependencyContext> _dependencies;

        private sealed class BootstrapContext : ContextLink<IContext> {
            public BootstrapContext(int sceneId, IContext context) : base(sceneId, context) { }
        }

        private sealed class DependencyContext : ContextLink<DependencyContainer> {
            public DependencyContext(int sceneId, DependencyContainer context) : base(sceneId, context) { }
        }

        [RuntimeInitializeOnLoadMethod]
        internal static void CreateContext() {
            GameObject context = new GameObject("ProjectContext");
            context.AddComponent<ProjectContext>();
        }

        private void Awake() {
            DontDestroyOnLoad(this);

            _contexts = new List<BootstrapContext>();
            _dependencies = new List<DependencyContext>();
            _loopContext = new LoopContext();

            _loopContext.Init();
        }

        private void Start() {
            InitScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            SceneManager.sceneLoaded += InitScene;
            SceneManager.sceneUnloaded += UnloadScene;
        }

        private void FixedUpdate() => _loopContext.FixedUpdate();

        private void Update() {
        #if UNITY_EDITOR
            ObservedTestUtility.Next();
        #endif

            _loopContext.Update();
        }

        private void LateUpdate() => _loopContext.LateUpdate();

        internal void AddContainer(int sceneId, DependencyContainer container) {
            if (_dependencies.TryGetContext(sceneId, out DependencyContainer _, out int _)) {
                return;
            }

            _dependencies.Add(new DependencyContext(sceneId, container));
        }

        internal void AddFixedTicks(int sceneId, List<IFixedTick> ticks) => _loopContext.AddFixedTicks(sceneId, ticks);

        internal void AddTicks(int sceneId, List<ITick> ticks) => _loopContext.AddTicks(sceneId, ticks);

        internal void AddLateTicks(int sceneId, List<ILateTick> ticks) => _loopContext.AddLateTicks(sceneId, ticks);

        internal void Resolve(IResolving resolving) => ResolveUtility.Resolve(resolving, CreateContainer());

        internal void Resolve(List<IResolving> resolving) => ResolveUtility.Resolve(resolving, CreateContainer());

        internal void ResolveWithoutApply(IResolving resolving, List<IDependency> dependencies) {
            ResolveUtility.ResolveWithoutApply(resolving, CreateContainer(dependencies));
        }

        internal void ResolveWithoutApply(List<IResolving> resolving, List<IDependency> dependencies) {
            ResolveUtility.ResolveWithoutApply(resolving, CreateContainer(dependencies));
        }

        internal void ConnectLoop(int sceneId, ILoop loop) => _loopContext.ConnectLoop(sceneId, loop);
        
        internal void DisconnectLoop(int sceneId, ILoop loop) => _loopContext.DisconnectLoop(sceneId, loop);

        private DependencyContainer CreateContainer(List<IDependency> dependencies) {
            DependencyContainer container = CreateContainer();

            foreach (IDependency dependency in dependencies) {
                container.dependencies.Add(dependency.GetType(), dependency);
            }

            return container;
        }

        private DependencyContainer CreateContainer() {
            DependencyContainer container = new DependencyContainer(64);

            foreach (DependencyContext context in _dependencies) {
                foreach (KeyValuePair<Type, IDependency> contextDependencies in context.context.dependencies) {
                    container.dependencies.Add(contextDependencies.Key, contextDependencies.Value);
                }
            }

            return container;
        }

        private void InitScene(Scene scene, LoadSceneMode mode) {
            int sceneId = scene.buildIndex;

            if (_contexts.TryGetContext(sceneId, out IContext _, out int _)) {
                return;
            }

            if (!TryFindSceneContext(scene.GetRootGameObjects(), out IContext context)) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError($"ProjectContext.InitScene() - {scene.name} not contain Bootstrap!");
            #endif
                return;
            }

            context.Create();
            context.Init(this, sceneId);

            _contexts.Add(new BootstrapContext(sceneId, context));
        }

        private bool TryFindSceneContext(GameObject[] rootObjects, out IContext context) {
            for (int rootId = 0; rootId < rootObjects.Length; rootId++) {
                context = rootObjects[rootId].GetComponent<IContext>();

                if (context == null) {
                    continue;
                }

                return true;
            }

            context = null;

            return false;
        }

        private void UnloadScene(Scene scene) {
            int sceneId = scene.buildIndex;

            if (!_contexts.TryGetContext(sceneId, out IContext context, out int contextId)) {
                return;
            }

            if (context is IGlobalContext) {
                return;
            }

            context.Unload();
            
            if (_dependencies.TryGetContext(sceneId, out DependencyContainer _, out int modelsId)) {
                _dependencies.RemoveAt(modelsId);
            }

            _loopContext.RemoveAllContextsWithId(sceneId);
            _contexts.RemoveAt(contextId);
        }
    }
}