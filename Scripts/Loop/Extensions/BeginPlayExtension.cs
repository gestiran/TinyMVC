using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Debugging;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Unity.Profiling;
using Unity.Profiling.LowLevel;
using UnityEngine;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class BeginPlayExtension {
        public static void TryBeginPlay<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IBeginPlayAsync otherAsync) {
                    DebugUtility.CheckAndLogException(otherAsync.BeginPlay, objects[objId]);
                } else if (objects[objId] is IBeginPlay other) {
                    DebugUtility.CheckAndLogException(other.BeginPlay, objects[objId]);
                }
            }
        }
        
        public static async Task TryBeginPlayAsync<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IBeginPlayAsync otherAsync) {
                    await DebugUtility.CheckAndLogExceptionAsync(otherAsync.BeginPlay, objects[objId]);
                } else if (objects[objId] is IBeginPlay other) {
                    DebugUtility.CheckAndLogException(other.BeginPlay, objects[objId]);
                }
            }
        }
        
        public static void TryBeginPlay<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IBeginPlayAsync otherAsync) {
                    DebugUtility.CheckAndLogException(otherAsync.BeginPlay, objects[objId]);
                } else if (objects[objId] is IBeginPlay other) {
                    DebugUtility.CheckAndLogException(other.BeginPlay, objects[objId]);
                }
            }
        }
        
        public static async Task TryBeginPlayAsync<T>(this List<T> objects) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            ProfilerMarker group = new ProfilerMarker(ProfilerCategory.Scripts, "TinyMVC.BeginPlay", MarkerFlags.Script);
            
            group.Begin();
            #endif
            
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IBeginPlayAsync otherAsync) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    
                    ProfilerMarker profile = new ProfilerMarker(ProfilerCategory.Scripts, $"{otherAsync.GetType().Name}", MarkerFlags.Script);
                    
                    if (otherAsync is Object unityObject) {
                        profile.Begin(unityObject);
                    } else {
                        profile.Begin();
                    }
                    
                    await DebugUtility.CheckAndLogExceptionAsync(otherAsync.BeginPlay, objects[objId]);
                    
                    profile.End();
                    
                    #else
                    await await DebugUtility.CheckAndLogExceptionAsync(otherAsync.BeginPlay, objects[objId]);
                    #endif
                } else if (objects[objId] is IBeginPlay other) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    
                    ProfilerMarker profile = new ProfilerMarker(ProfilerCategory.Scripts, $"{other.GetType().Name}", MarkerFlags.Script);
                    
                    if (other is Object unityObject) {
                        profile.Begin(unityObject);
                    } else {
                        profile.Begin();
                    }
                    
                    DebugUtility.CheckAndLogException(other.BeginPlay, objects[objId]);
                    
                    profile.End();
                    
                    #else
                    DebugUtility.CheckAndLogException(other.BeginPlay, objects[objId]);
                    #endif
                }
            }
            
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            group.End();
            #endif
        }
        
        public static void BeginPlay<TBeginPlay>(this TBeginPlay[] objects) where TBeginPlay : IBeginPlay {
            for (int objId = 0; objId < objects.Length; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].BeginPlay, objects[objId]);
            }
        }
        
        public static async Task BeginPlayAsync<TBeginPlay>(this TBeginPlay[] objects) where TBeginPlay : IBeginPlayAsync {
            for (int objId = 0; objId < objects.Length; objId++) {
                await DebugUtility.CheckAndLogExceptionAsync(objects[objId].BeginPlay, objects[objId]);
            }
        }
        
        public static void BeginPlay<TBeginPlay>(this List<TBeginPlay> objects) where TBeginPlay : IBeginPlay {
            for (int objId = 0; objId < objects.Count; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].BeginPlay, objects[objId]);
            }
        }
        
        public static async Task BeginPlayAsync<TBeginPlay>(this List<TBeginPlay> objects) where TBeginPlay : IBeginPlayAsync {
            for (int objId = 0; objId < objects.Count; objId++) {
                await DebugUtility.CheckAndLogExceptionAsync(objects[objId].BeginPlay, objects[objId]);
            }
        }
    }
}