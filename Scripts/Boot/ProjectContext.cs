using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Contexts;
using TinyMVC.Boot.Helpers;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TinyMVC.Boot {
    public static class ProjectContext {
        public static ProjectData data { get; private set; }
        
        private static Dictionary<int, SceneContext> _contexts;
        private static LoopContext _loopContext;
        
        private const int _NULL_SCENE_ID = 1;
        
        public static async void LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single) {
            if (mode == LoadSceneMode.Single) {
                UnloadScene(SceneManager.GetActiveScene().buildIndex);
                await Task.Yield();
            }
            
            SceneManager.LoadScene(sceneBuildIndex, mode);
        }
        
        public static async void ChangeScene(int sceneBuildIndex) {
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
        
        public static bool TryGetGlobalUnload(out UnloadPool unload) => TryGetGlobalUnload(SceneManager.GetActiveScene().buildIndex, out unload);
        
        public static bool TryGetGlobalUnload(int sceneId, out UnloadPool unload) {
            if (_contexts.TryGetValue(sceneId, out SceneContext context)) {
                unload = context.unload;
                
                return true;
            }
            
            unload = null;
            return false;
        }
        
    #if UNITY_EDITOR
        
        public static void LoadScene_Editor(string path, LoadSceneMode mode = LoadSceneMode.Single) {
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
            data = new ProjectData();
            
            Init();
            BeginPlay();
            
            LoopComponent.Create(Tick, FixedTick, LateTick);
        }
        
        private static void Init() {
            _contexts = new Dictionary<int, SceneContext>();
            _loopContext = new LoopContext();
            
            _loopContext.Init();
            
            Application.quitting += Unload;
        }
        
        private static void BeginPlay() {
            InitScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            
            SceneManager.sceneLoaded += InitScene;
        }
        
        private static void Unload() {
            _contexts = null;
            _loopContext = null;
            
            SceneManager.sceneLoaded -= InitScene;
        }
        
        private static void FixedTick() => _loopContext.FixedTick();
        
        private static void Tick() {
            TimelineUtility.Next();
            _loopContext.Tick();
        }
        
        private static void LateTick() => _loopContext.LateTick();
        
        internal static void AddFixedTicks(int sceneId, List<IFixedTick> ticks) => _loopContext.AddFixedTicks(sceneId, ticks);
        
        internal static void AddTicks(int sceneId, List<ITick> ticks) => _loopContext.AddTicks(sceneId, ticks);
        
        internal static void AddLateTicks(int sceneId, List<ILateTick> ticks) => _loopContext.AddLateTicks(sceneId, ticks);
        
        internal static void ConnectLoop(int sceneId, ILoop loop) => _loopContext.ConnectLoop(sceneId, loop);
        
        internal static void DisconnectLoop(int sceneId, ILoop loop) => _loopContext.DisconnectLoop(sceneId, loop);
        
        private static async void InitScene(Scene scene, LoadSceneMode mode) {
            int sceneId = scene.buildIndex;
            
            if (_contexts.ContainsKey(sceneId)) {
                return;
            }
            
            if (!TryFindSceneContext(scene.GetRootGameObjects(), out SceneContext context)) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"ProjectContext.InitScene() - {scene.name} not contain Bootstrap!");
            #endif
                return;
            }
            
            _contexts.Add(sceneId, context);
            context.Create();
            
            await context.InitAsync(sceneId);
        }
        
        private static bool TryFindSceneContext(GameObject[] rootObjects, out SceneContext context) {
            for (int rootId = 0; rootId < rootObjects.Length; rootId++) {
                context = rootObjects[rootId].GetComponent<SceneContext>();
                
                if (context == null) {
                    continue;
                }
                
                return true;
            }
            
            context = null;
            return false;
        }
        
        private static void UnloadScene(int sceneBuildId) {
            if (!_contexts.TryGetValue(sceneBuildId, out SceneContext context)) {
                return;
            }
            
            if (context is IGlobalContext) {
                return;
            }
            
            context.Unload();
            
            RemoveGroupAttribute attribute = (RemoveGroupAttribute)Attribute.GetCustomAttribute(context.GetType(), typeof(RemoveGroupAttribute));
            
            if (attribute != null) {
                data.Remove(attribute.groups);
            }
            
            context.unload.Unload();
            
            _loopContext.RemoveAllContextsWithId(sceneBuildId);
            _contexts.Remove(sceneBuildId);
        }
    }
}