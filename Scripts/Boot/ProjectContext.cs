using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using TinyMVC.Boot.Contexts;
using TinyMVC.Loop;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TinyMVC.Boot {
    public static class ProjectContext {
        public static ProjectData data { get; private set; }
        public static SceneContext activeContext { get; private set; }
        
        private static Dictionary<string, SceneContext> _contexts;
        private static Dictionary<int, List<SceneContext>> _sceneContexts;
        
        public static IEnumerable<SceneContext> Contexts() {
            foreach (SceneContext context in _contexts.Values) {
                yield return context;
            }
        }
        
        public static bool TryGetContext(string contextKey, out SceneContext context) => _contexts.TryGetValue(contextKey, out context);
        
        public static async UniTask LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single) {
            if (mode == LoadSceneMode.Single) {
                RemoveContexts(SceneManager.GetActiveScene().buildIndex);
                await UniTask.Yield();
            }
            
            AsyncOperation loading = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
            
            if (loading == null) {
                Debug.LogError("Unity internal load scene error!");
                return;
            }
            
            while (loading.isDone == false) {
                await UniTask.Yield();
            }
            
            await UniTask.Delay(100, DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update);
            
            AsyncOperation clean = Resources.UnloadUnusedAssets();
            
            while (clean.isDone == false) {
                await UniTask.Yield();
            }
        }
        
        public static async UniTask LoadScene(int sceneBuildIndex, Action<float> progress, LoadSceneMode mode = LoadSceneMode.Single) {
            if (mode == LoadSceneMode.Single) {
                RemoveContexts(SceneManager.GetActiveScene().buildIndex);
                await UniTask.Yield();
            }
            
            AsyncOperation loading = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
            
            if (loading == null) {
                Debug.LogError("Unity internal load scene error!");
                return;
            }
            
            while (loading.isDone == false) {
                progress.Invoke(loading.progress - 0.5f);
                await UniTask.Yield();
            }
            
            await UniTask.Delay(100, DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update);
            
            AsyncOperation clean = Resources.UnloadUnusedAssets();
            
            while (clean.isDone == false) {
                progress.Invoke(0.5f + (clean.progress - 0.5f));
                await UniTask.Yield();
            }
        }
        
        public static bool TryGetGlobalUnload(out UnloadPool unload) => TryGetGlobalUnload(activeContext.key, out unload);
        
        public static bool TryGetGlobalUnload(string contextKey, out UnloadPool unload) {
            if (_contexts.TryGetValue(contextKey, out SceneContext context)) {
                unload = context.unload;
                return true;
            }
            
            unload = null;
            return false;
        }
        
    #if UNITY_EDITOR
        
        public static void LoadScene_Editor(string path, LoadSceneMode mode = LoadSceneMode.Single) {
            LoadSceneParameters parameters = new LoadSceneParameters(mode, LocalPhysicsMode.None);
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path, parameters);
        }
        
    #endif
        
        /// <summary> First project context creating </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void CreateContext() {
            data = new ProjectData();
            
            _contexts = new Dictionary<string, SceneContext>();
            _sceneContexts = new Dictionary<int, List<SceneContext>>();
        }
        
        internal static void AddFixedTicks(string contextKey, List<IFixedTick> ticks) {
            if (_contexts.TryGetValue(contextKey, out SceneContext context)) {
                context.fixedTicks.AddRange(ticks);
            }
        }
        
        internal static void AddTicks(string contextKey, List<ITick> ticks) {
            if (_contexts.TryGetValue(contextKey, out SceneContext context)) {
                context.ticks.AddRange(ticks);
            }
        }
        
        internal static void AddLateTicks(string contextKey, List<ILateTick> ticks) {
            if (_contexts.TryGetValue(contextKey, out SceneContext context)) {
                context.lateTicks.AddRange(ticks);
            }
        }
        
        internal static void ConnectLoop(string contextKey, ILoop loop) {
            if (_contexts.TryGetValue(contextKey, out SceneContext context)) {
                context.ConnectLoop(loop);
            }
        }
        
        internal static void DisconnectLoop(string contextKey, ILoop loop) {
            if (_contexts.TryGetValue(contextKey, out SceneContext context)) {
                context.DisconnectLoop(loop);
            }
        }
        
        internal static async void AddContext<T>(T context, int sceneId) where T : SceneContext {
            if (_contexts.TryAdd(context.key, context) == false) {
                return;
            }
            
            if (_sceneContexts.TryGetValue(sceneId, out List<SceneContext> list)) {
                if (list.Contains(context) == false) {
                    list.Add(context);
                }
            } else {
                _sceneContexts.Add(sceneId, new List<SceneContext>() { context });
            }
            
            activeContext = context;
            context.Create();
            await context.InitAsync();
        }
        
        internal static void RemoveContext<T>(T context, int sceneId) where T : SceneContext {
            if (context is IGlobalContext) {
                return;
            }
            
            if (_sceneContexts.TryGetValue(sceneId, out List<SceneContext> list)) {
                if (list.Contains(context)) {
                    list.Remove(context);
                }
            }
            
            context.Unload();
            context.unload.Unload();
            
            _contexts.Remove(context.key);
            data.Remove(context.key);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RemoveContexts(int sceneBuildIndex) {
            if (_sceneContexts.TryGetValue(sceneBuildIndex, out List<SceneContext> contexts) == false) {
                return;
            }
            
            SceneContext[] contextsArray = contexts.ToArray();
            
            for (int contextId = 0; contextId < contextsArray.Length; contextId++) {
                contextsArray[contextId].Remove();
            }
            
            contexts.Clear();
        }
    }
}