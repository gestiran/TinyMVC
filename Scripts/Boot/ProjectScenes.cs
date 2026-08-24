// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TinyMVC.Boot {
    /// <summary> Unity scenes management adapter over the context registry (<see cref="ProjectContext"/>). </summary>
    public static class ProjectScenes {
        private const int _LOAD_ITERATION = 250;
        
        public static async UniTask LoadScene(int sceneBuildIndex, bool clearAssets = false) {
            int currentSceneId = SceneManager.GetActiveScene().buildIndex;
            
            await ProjectContext.RemoveContexts(currentSceneId);
            
            await UniTask.Delay(_LOAD_ITERATION, true);
            
            if (currentSceneId == sceneBuildIndex || clearAssets) {
                SceneManager.CreateScene("Null");
                
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
        internal static void CreateContext() => ProjectContext.Initialize();
    }
}