using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Binding;
using TinyMVC.Boot.Contexts;
using TinyMVC.Boot.Helpers;
using TinyMVC.Loop;
using TinyMVC.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TinyMVC.Debugging;
using Unity.Profiling;
#endif

namespace TinyMVC.Boot {
    /// <summary> Global initialization context, auto created on any scene at start game </summary>
    public sealed class ProjectContext {
        public static float deltaTime { get; internal set; }
        public static float unscaledDeltaTime { get; internal set; }
        
        public static BindAPI binding { get; private set; }
        public static ProjectData data { get; private set; }
        public static ProjectContext current { get; private set; }
        
        /// <summary> Boot contexts list </summary>
        /// <remarks> Scene can contain one or more <see cref="IContext"/> initializers, always, they will be invoked on <see cref="CreateContext"/> when scene is loaded </remarks>
        private List<BootstrapContext> _contexts;
        
        /// <summary> Controls updating process </summary>
        /// <remarks> Run <see cref="Tick"/>, <see cref="FixedTick"/>, <see cref="LateTick"/> all scenes </remarks>
        private LoopContext _loopContext;
        
        private const int _NULL_SCENE_ID = 1;
        
        static ProjectContext() {
            float delta = Time.unscaledDeltaTime;
            deltaTime = delta * Time.timeScale;
            unscaledDeltaTime = delta;
        }
        
        private sealed class BootstrapContext : ContextLink<IContext> {
            public readonly UnloadPool unload;
            private readonly SceneCoroutines _coroutines;
            
            public BootstrapContext(int sceneId, IContext context) : base(sceneId, context) {
                unload = new UnloadPool();
                _coroutines = new GameObject(nameof(SceneCoroutines)).AddComponent<SceneCoroutines>();
            }
            
            public Coroutine StartCoroutine(IEnumerator enumerator) => _coroutines.AddCoroutine(enumerator);
            
            public void StopCoroutine(Coroutine coroutine) => _coroutines.RemoveCoroutine(coroutine);
            
            public void StopAllCoroutines() => _coroutines.StopAll();
        }
        
        public async void LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single) {
            if (mode == LoadSceneMode.Single) {
                UnloadScene(SceneManager.GetActiveScene().buildIndex);
                await Task.Yield();
            }
            
            SceneManager.LoadScene(sceneBuildIndex, mode);
        }
        
        public async void ChangeScene(int sceneBuildIndex) {
            int currentSceneId = SceneManager.GetActiveScene().buildIndex;
            
            UnloadScene(currentSceneId);
            
            AsyncOperation loadingNull = SceneManager.LoadSceneAsync(_NULL_SCENE_ID, LoadSceneMode.Additive);
            
            if (loadingNull == null) {
                Debug.LogError("Unity internal load scene error!");
                return;
            }
            
            while (loadingNull.isDone == false) {
                await Task.Yield();
            }
            
            AsyncOperation unloading = SceneManager.UnloadSceneAsync(currentSceneId, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            
            if (unloading == null) {
                Debug.LogError("Unity internal unload scene error!");
                return;
            }
            
            while (unloading.isDone == false) {
                await Task.Yield();
            }
            
            UnloadScene(_NULL_SCENE_ID);
            
            AsyncOperation clean = Resources.UnloadUnusedAssets();
            
            while (clean.isDone == false) {
                await Task.Yield();
            }
            
            AsyncOperation loading = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);
            
            if (loading == null) {
                Debug.LogError("Unity internal load scene error!");
                return;
            }
            
            while (loading.isDone == false) {
                await Task.Yield();
            }
            
            await Task.Delay(100);
        }
        
        public Coroutine StartCoroutine(IEnumerator enumerator) => StartCoroutine(SceneManager.GetActiveScene().buildIndex, enumerator);
        
        public Coroutine StartCoroutine(int sceneId, IEnumerator enumerator) {
            if (_contexts.TryGetContext(sceneId, out IContext _, out int id)) {
                return _contexts[id].StartCoroutine(enumerator);
            }
            
            return null;
        }
        
        public void StopCoroutine(Coroutine coroutine) => StopCoroutine(SceneManager.GetActiveScene().buildIndex, coroutine);
        
        public void StopCoroutine(int sceneId, Coroutine coroutine) {
            if (_contexts.TryGetContext(sceneId, out IContext _, out int id)) {
                _contexts[id].StopCoroutine(coroutine);
            }
        }
        
        public bool TryGetGlobalUnload(out UnloadPool unload) => TryGetGlobalUnload(SceneManager.GetActiveScene().buildIndex, out unload);
        
        public bool TryGetGlobalUnload(int sceneId, out UnloadPool unload) {
            if (_contexts.TryGetContext(sceneId, out IContext _, out int id)) {
                unload = _contexts[id].unload;
                
                return true;
            }
            
            unload = null;
            
            return false;
        }
        
        #if UNITY_EDITOR
        
        public void LoadSceneEditor(string path, LoadSceneMode mode = LoadSceneMode.Single) {
            if (mode == LoadSceneMode.Single) {
                UnloadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
            LoadSceneParameters parameters = new LoadSceneParameters(mode, LocalPhysicsMode.None);
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path, parameters);
        }
        
        #endif
        
        /// <summary> First project context creating </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        internal static void CreateContext() {
            binding = new BindAPI();
            data = new ProjectData();
            current = new ProjectContext();
            
            current.Init();
            current.BeginPlay();
            
            LoopComponent.Create(current.Tick, current.FixedTick, current.LateTick);
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
            TimelineUtility.Next();
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
        
        internal void ConnectLoop(int sceneId, ILoop loop) => _loopContext.ConnectLoop(sceneId, loop);
        
        internal void DisconnectLoop(int sceneId, ILoop loop) => _loopContext.DisconnectLoop(sceneId, loop);
        
        private async void InitScene(Scene scene, LoadSceneMode mode) {
            int sceneId = scene.buildIndex;
            
            if (_contexts.TryGetContext(sceneId, out IContext _, out int _)) {
                return;
            }
            
            if (!TryFindSceneContext(scene.GetRootGameObjects(), out IContext context)) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"ProjectContext.InitScene() - {scene.name} not contain Bootstrap!");
                #endif
                return;
            }
            
            _contexts.Add(new BootstrapContext(sceneId, context));
            context.Create();
            
            await context.InitAsync(this, sceneId);
        }
        
        private bool TryFindSceneContext(GameObject[] rootObjects, out IContext contexts) {
            for (int rootId = 0; rootId < rootObjects.Length; rootId++) {
                contexts = rootObjects[rootId].GetComponent<IContext>();
                
                if (contexts == null) {
                    continue;
                }
                
                return true;
            }
            
            contexts = null;
            
            return false;
        }
        
        private void UnloadScene(int buildId) {
            if (!_contexts.TryGetContext(buildId, out IContext context, out int contextId)) {
                return;
            }
            
            if (context is IGlobalContext) {
                return;
            }
            
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            ProfilerMarker contextCreate = new ProfilerMarker(ProfilerCategory.Scripts, "Context: Unload");
            contextCreate.Begin();
            #endif
            
            context.Unload();
            
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            contextCreate.End();
            #endif
            
            data.Remove(buildId);
            
            _contexts[contextId].unload.Unload();
            _contexts[contextId].StopAllCoroutines();
            
            _loopContext.RemoveAllContextsWithId(buildId);
            _contexts.RemoveAt(contextId);
        }
    }
}