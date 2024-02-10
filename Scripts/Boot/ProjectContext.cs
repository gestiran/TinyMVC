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

#if UNITY_EDITOR && GES_MVC_PROFILING
using Unity.Profiling;
#endif

namespace TinyMVC.Boot {
    /// <summary> Global initialization context, auto created on any scene at start game </summary>
    [DisallowMultipleComponent]
    internal sealed class ProjectContext : MonoBehaviour {
        /// <summary> Boot contexts list </summary>
        /// <remarks> Scene can contain one or more <see cref="IContext"/> initializers, always, they will be invoked on <see cref="CreateContext"/> when scene is loaded </remarks>
        private List<BootstrapContext> _contexts;
        
        /// <summary> Controls updating process </summary>
        /// <remarks> Run <see cref="Update"/>, <see cref="FixedUpdate"/>, <see cref="LateUpdate"/> all scenes </remarks>
        private LoopContext _loopContext;

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [BoxGroup("Debug"), ShowInInspector, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowFoldout = false)]
    #endif
        private List<DependencyContext> _dependencies;

    #if UNITY_EDITOR && GES_MVC_PROFILING
        private ProfilerMarker _fixedUpdateMarker;
        private ProfilerMarker _updateMarker;
        private ProfilerMarker _lateUpdateMarker;
    #endif
        
        private sealed class BootstrapContext : ContextLink<IContext[]> {
            public BootstrapContext(int sceneId, IContext[] context) : base(sceneId, context) { }
        }

        private sealed class DependencyContext : ContextLink<DependencyContainer> {
            public DependencyContext(int sceneId, DependencyContainer context) : base(sceneId, context) { }
        }

        /// <summary> First project context creating </summary>
        [RuntimeInitializeOnLoadMethod]
        internal static void CreateContext() {
            GameObject context = new GameObject("ProjectContext");
            context.AddComponent<ProjectContext>();
        }

        private void Awake() {
        #if UNITY_EDITOR && GES_MVC_PROFILING
            _fixedUpdateMarker = new ProfilerMarker(ProfilerCategory.Scripts, "Project.FixedUpdate");
            _updateMarker = new ProfilerMarker(ProfilerCategory.Scripts, "Project.Update");
            _lateUpdateMarker = new ProfilerMarker(ProfilerCategory.Scripts, "Project.LateUpdate");
        #endif
            
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

        private void FixedUpdate() {
        #if UNITY_EDITOR && GES_MVC_PROFILING
            _fixedUpdateMarker.Begin();
        #endif
            
            _loopContext.FixedUpdate();
            
        #if UNITY_EDITOR && GES_MVC_PROFILING
            _fixedUpdateMarker.End();
        #endif
        }

        private void Update() {
        #if UNITY_EDITOR
            ObservedTestUtility.Next();
        #endif
            
        #if UNITY_EDITOR && GES_MVC_PROFILING
            _updateMarker.Begin();
        #endif

            _loopContext.Update();
            
        #if UNITY_EDITOR && GES_MVC_PROFILING
            _updateMarker.End();
        #endif
        }

        private void LateUpdate() {
        #if UNITY_EDITOR && GES_MVC_PROFILING
            _lateUpdateMarker.Begin();
        #endif
            
            _loopContext.LateUpdate();
            
        #if UNITY_EDITOR && GES_MVC_PROFILING
            _lateUpdateMarker.End();
        #endif
        }

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

        internal void ConnectLoop(int sceneId, ILoop loop) {
        #if UNITY_EDITOR && GES_MVC_PROFILING
            ProfilerMarker initScene;

            if (sceneId < 0) {
                initScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.ConnectLoop(ID: {sceneId:00})");
            } else {
                initScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.ConnectLoop(Scene: {SceneManager.GetSceneByBuildIndex(sceneId).name})");
            }
            
            initScene.Begin();
        #endif
            
            _loopContext.ConnectLoop(sceneId, loop);
            
        #if UNITY_EDITOR && GES_MVC_PROFILING
            initScene.End();
        #endif
        }

        internal void DisconnectLoop(int sceneId, ILoop loop) {
        #if UNITY_EDITOR && GES_MVC_PROFILING
            ProfilerMarker initScene;

            if (sceneId < 0) {
                initScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.DisconnectLoop(ID: {sceneId:00})");
            } else {
                initScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.DisconnectLoop(Scene: {SceneManager.GetSceneByBuildIndex(sceneId).name})");
            }
            
            initScene.Begin();
        #endif
            
            _loopContext.DisconnectLoop(sceneId, loop);
            
        #if UNITY_EDITOR && GES_MVC_PROFILING
            initScene.End();
        #endif
        }

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

            if (_contexts.TryGetContext(sceneId, out IContext[] _, out int _)) {
                return;
            }

            if (!TryFindSceneContext(scene.GetRootGameObjects(), out IContext[] contexts)) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"ProjectContext.InitScene() - {scene.name} not contain Bootstrap!");
            #endif
                return;
            }

        #if UNITY_EDITOR && GES_MVC_PROFILING
            ProfilerMarker initScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.InitScene(Scene: {scene.name})");
            initScene.Begin();
        #endif

            for (int contextId = contexts.Length - 1; contextId >= 0; contextId--) {
            #if UNITY_EDITOR && GES_MVC_PROFILING
                ProfilerMarker contextCreate = new ProfilerMarker(ProfilerCategory.Scripts, "Context: Create");
                contextCreate.Begin();
            #endif

                contexts[contextId].Create();

            #if UNITY_EDITOR && GES_MVC_PROFILING
                contextCreate.End();
                ProfilerMarker contextInit = new ProfilerMarker(ProfilerCategory.Scripts, "Context: Init");
                contextInit.Begin();
            #endif

                contexts[contextId].Init(this, sceneId);

            #if UNITY_EDITOR && GES_MVC_PROFILING
                contextInit.End();
            #endif
            }

        #if UNITY_EDITOR && GES_MVC_PROFILING
            initScene.End();
        #endif

            _contexts.Add(new BootstrapContext(sceneId, contexts));
        }

        private bool TryFindSceneContext(GameObject[] rootObjects, out IContext[] contexts) {
            for (int rootId = 0; rootId < rootObjects.Length; rootId++) {
                contexts = rootObjects[rootId].GetComponents<IContext>();

                if (contexts.Length <= 0) {
                    continue;
                }

                return true;
            }

            contexts = null;

            return false;
        }

        private void UnloadScene(Scene scene) {
            int sceneId = scene.buildIndex;

            if (!_contexts.TryGetContext(sceneId, out IContext[] contexts, out int contextId)) {
                return;
            }

            for (int i = 0; i < contexts.Length; i++) {
                if (contexts[i] is IGlobalContext) {
                    return;
                }
            }

        #if UNITY_EDITOR && GES_MVC_PROFILING
            ProfilerMarker unloadScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.UnloadScene(Scene: {scene.name})");
            unloadScene.Begin();
        #endif
            
            for (int i = contexts.Length - 1; i >= 0; i--) {
            #if UNITY_EDITOR && GES_MVC_PROFILING
                ProfilerMarker contextCreate = new ProfilerMarker(ProfilerCategory.Scripts, "Context: Unload");
                contextCreate.Begin();
            #endif
                
                contexts[i].Unload();
                
            #if UNITY_EDITOR && GES_MVC_PROFILING
                contextCreate.End();
            #endif
            }
            
        #if UNITY_EDITOR && GES_MVC_PROFILING
            unloadScene.End();
        #endif

            if (_dependencies.TryGetContext(sceneId, out DependencyContainer _, out int modelsId)) {
                _dependencies.RemoveAt(modelsId);
            }

            _loopContext.RemoveAllContextsWithId(sceneId);
            _contexts.RemoveAt(contextId);
        }
    }
}