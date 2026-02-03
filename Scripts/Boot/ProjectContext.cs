// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TinyMVC.Boot.Contexts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TinyMVC.Boot {
    public static class ProjectContext {
        public static ProjectComponents components { get; private set; }
        public static ProjectData data { get; private set; }
        public static SceneContext scene { get; private set; }
        
        private static Dictionary<string, SceneContext> _contexts;
        private static Dictionary<int, List<SceneContext>> _sceneContexts;
        
        private const int _NULL_SCENE_ID = 1;
        private const int _LOAD_ITERATION = 250;
        
        public static IEnumerable<SceneContext> Contexts() {
            foreach (SceneContext context in _contexts.Values) {
                yield return context;
            }
        }
        
        public static bool TryGetContext(string contextKey, out SceneContext context) => _contexts.TryGetValue(contextKey, out context);
        
        public static async UniTask LoadScene(int sceneBuildIndex, bool clearAssets = false) {
            int currentSceneId = SceneManager.GetActiveScene().buildIndex;
            
            RemoveContexts(currentSceneId);
            
            await UniTask.Delay(_LOAD_ITERATION, true);
            
            if (currentSceneId == sceneBuildIndex || clearAssets) {
                AsyncOperation loadNull = SceneManager.LoadSceneAsync(_NULL_SCENE_ID, LoadSceneMode.Additive);
                
                if (loadNull == null) {
                    Debug.LogError("Unity internal load scene error!");
                    return;
                }
                
                do {
                    await UniTask.Delay(_LOAD_ITERATION, true);
                } while (loadNull.isDone == false);
                
                AsyncOperation unloadCurrent = SceneManager.UnloadSceneAsync(currentSceneId);
                
                if (unloadCurrent == null) {
                    Debug.LogError("Unity internal unload scene error!");
                    return;
                }
                
                do {
                    await UniTask.Delay(_LOAD_ITERATION, true);
                } while (unloadCurrent.isDone == false);
                
                if (clearAssets) {
                    AsyncOperation unloadAssets = Resources.UnloadUnusedAssets();
                    
                    do {
                        await UniTask.Delay(_LOAD_ITERATION, true);
                    } while (unloadAssets.isDone == false);
                }
            }
            
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);
            
            if (loadScene == null) {
                Debug.LogError("Unity internal load scene error!");
                return;
            }
            
            do {
                await UniTask.Delay(_LOAD_ITERATION, true);
            } while (loadScene.isDone == false);
        }
        
        public static async UniTask AddScene(int sceneBuildIndex) {
            AsyncOperation loading = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
            
            if (loading == null) {
                Debug.LogError("Unity internal load scene error!");
                return;
            }
            
            do {
                await UniTask.Delay(_LOAD_ITERATION, true);
            } while (loading.isDone == false);
        }
        
        public static async UniTask RemoveScene(int sceneBuildIndex) {
            AsyncOperation loading = SceneManager.UnloadSceneAsync(sceneBuildIndex);
            
            if (loading == null) {
                Debug.LogError("Unity internal unload scene error!");
                return;
            }
            
            do {
                await UniTask.Delay(_LOAD_ITERATION, true);
            } while (loading.isDone == false);
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
            components = new ProjectComponents();
            data = new ProjectData(components);
            
            _contexts = new Dictionary<string, SceneContext>();
            _sceneContexts = new Dictionary<int, List<SceneContext>>();
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
            
            scene = context;
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
            
            _contexts.Remove(context.key);
            data.Remove(context.key);
        }
        
        private static void RemoveContexts(int sceneBuildIndex) {
            if (_sceneContexts.TryGetValue(sceneBuildIndex, out List<SceneContext> contexts) == false) {
                return;
            }
            
            SceneContext[] contextsArray = contexts.ToArray();
            
            for (int contextId = 0; contextId < contextsArray.Length; contextId++) {
                contextsArray[contextId].Remove();
            }
            
            contexts.Clear();
            
            _sceneContexts.Remove(sceneBuildIndex);
        }
    }
}