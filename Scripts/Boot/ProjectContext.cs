using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Binding;
using TinyMVC.Boot.Contexts;
using TinyMVC.Boot.Helpers;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TinyMVC.Debugging;
using Unity.Profiling;
#endif

namespace TinyMVC.Boot {
    /// <summary> Global initialization context, auto created on any scene at start game </summary>
    public sealed class ProjectContext {
        /// <summary> Boot contexts list </summary>
        /// <remarks> Scene can contain one or more <see cref="IContext"/> initializers, always, they will be invoked on <see cref="CreateContext"/> when scene is loaded </remarks>
        private List<BootstrapContext> _contexts;

        /// <summary> Controls updating process </summary>
        /// <remarks> Run <see cref="Tick"/>, <see cref="FixedTick"/>, <see cref="LateTick"/> all scenes </remarks>
        private LoopContext _loopContext;

        public static BindAPI binding { get; private set; }
        public static ProjectData data { get; private set; }
        public static ProjectContext current { get; private set; }

        private sealed class BootstrapContext : ContextLink<IContext[]> {
            public readonly UnloadPool unload;

            public BootstrapContext(int sceneId, IContext[] context) : base(sceneId, context) {
                unload = new UnloadPool();
            }
        }
        
        public async void LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single) {
            if (mode == LoadSceneMode.Single) {
                UnloadScene(SceneManager.GetActiveScene());
                await Task.Yield();
            }
            
            SceneManager.LoadScene(sceneBuildIndex, mode);
        }

        public bool TryGetGlobalUnload(out UnloadPool unload) => TryGetGlobalUnload(SceneManager.GetActiveScene().buildIndex, out unload);

        public bool TryGetGlobalUnload(int sceneId, out UnloadPool unload) {
            if (_contexts.TryGetContext(sceneId, out IContext[] _, out int id)) {
                unload = _contexts[id].unload;
                return true;
            }

            unload = null;
            return false;
        }

    #if UNITY_EDITOR
        
        public void LoadSceneEditor(string path, LoadSceneMode mode = LoadSceneMode.Single) {
            if (mode == LoadSceneMode.Single) {
                UnloadScene(SceneManager.GetActiveScene());
            }
            
            LoadSceneParameters parameters = new LoadSceneParameters(mode, LocalPhysicsMode.None);
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path, parameters);
        }
        
    #endif
        
        /// <summary> First project context creating </summary>
        [RuntimeInitializeOnLoadMethod]
        internal static void CreateContext() {
            binding = new BindAPI();
            data = new ProjectData();
            current = new ProjectContext();

            current.Init();
            current.BeginPlay();

            LoopUtility.ApplyLoop(current.Tick, current.FixedTick, current.LateTick);
        }

        private void Init() {
            _contexts = new List<BootstrapContext>();
            _loopContext = new LoopContext();

            _loopContext.Init();

            Application.quitting += Unload;
        }

        private void BeginPlay() {
            InitScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            SceneManager.sceneLoaded += InitScene;
        }

        private void Unload() {
            _contexts = null;
            _loopContext = null;
            
            SceneManager.sceneLoaded -= InitScene;
            current = null;
        }
        
        private void FixedTick() {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            DebugUtility.ProfilerMarkerScripts("Project.FixedTick", () => _loopContext.FixedTick());
        #else
            _loopContext.FixedTick();
        #endif
        }

        private void Tick() {
            ObservedUtility.Next();
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            DebugUtility.ProfilerMarkerScripts("Project.Tick", () => _loopContext.Tick());
        #else
            _loopContext.Tick();
        #endif
        }

        private void LateTick() {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            DebugUtility.ProfilerMarkerScripts("Project.LateTick", () => _loopContext.LateTick());
        #else
            _loopContext.LateTick();
        #endif
        }

        internal void AddFixedTicks(int sceneId, List<IFixedTick> ticks) => _loopContext.AddFixedTicks(sceneId, ticks);

        internal void AddTicks(int sceneId, List<ITick> ticks) => _loopContext.AddTicks(sceneId, ticks);

        internal void AddLateTicks(int sceneId, List<ILateTick> ticks) => _loopContext.AddLateTicks(sceneId, ticks);

        internal void ConnectLoop(int sceneId, ILoop loop) {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            ProfilerMarker initScene;

            if (sceneId < 0) {
                initScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.ConnectLoop(ID: {sceneId:00})");
            } else {
                initScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.ConnectLoop(Scene: {SceneManager.GetSceneByBuildIndex(sceneId).name})");
            }

            initScene.Begin();
        #endif

            _loopContext.ConnectLoop(sceneId, loop);

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            initScene.End();
        #endif
        }

        internal void DisconnectLoop(int sceneId, ILoop loop) {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            ProfilerMarker initScene;

            if (sceneId < 0) {
                initScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.DisconnectLoop(ID: {sceneId:00})");
            } else {
                initScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.DisconnectLoop(Scene: {SceneManager.GetSceneByBuildIndex(sceneId).name})");
            }

            initScene.Begin();
        #endif

            _loopContext.DisconnectLoop(sceneId, loop);

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            initScene.End();
        #endif
        }

        private async void InitScene(Scene scene, LoadSceneMode mode) {
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
            
            _contexts.Add(new BootstrapContext(sceneId, contexts));

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            ProfilerMarker initScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.InitScene(Scene: {scene.name})");
            initScene.Begin();
        #endif

            for (int contextId = contexts.Length - 1; contextId >= 0; contextId--) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                ProfilerMarker contextCreate = new ProfilerMarker(ProfilerCategory.Scripts, "Context: Create");
                contextCreate.Begin();
            #endif

                await contexts[contextId].Create();

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                contextCreate.End();
                ProfilerMarker contextInit = new ProfilerMarker(ProfilerCategory.Scripts, "Context: Init");
                contextInit.Begin();
            #endif

                await contexts[contextId].InitAsync(this, sceneId);

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                contextInit.End();
            #endif
            }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            initScene.End();
        #endif
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
            
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            ProfilerMarker unloadScene = new ProfilerMarker(ProfilerCategory.Scripts, $"Project.UnloadScene(Scene: {scene.name})");
            unloadScene.Begin();
        #endif

            for (int i = contexts.Length - 1; i >= 0; i--) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                ProfilerMarker contextCreate = new ProfilerMarker(ProfilerCategory.Scripts, "Context: Unload");
                contextCreate.Begin();
            #endif

                contexts[i].Unload();

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                contextCreate.End();
            #endif
            }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            unloadScene.End();
        #endif

            data.Remove(sceneId);

            _contexts[contextId].unload.Unload();
            
            _loopContext.RemoveAllContextsWithId(sceneId);
            _contexts.RemoveAt(contextId);
        }
    }
}