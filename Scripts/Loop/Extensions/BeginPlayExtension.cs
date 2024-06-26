﻿using System.Collections.Generic;
using System.Threading.Tasks;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TinyMVC.Debugging;
using TinyMVC.Debugging.Exceptions;
using Unity.Profiling;
using Unity.Profiling.LowLevel;
using UnityEngine;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class BeginPlayExtension {
        public static void TryBeginPlay<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IBeginPlayAsync otherAsync) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => otherAsync.BeginPlay(), exception => new BeginPlayAsyncException(otherAsync, exception));
                    #else
                    otherAsync.BeginPlay();
                    #endif
                } else if (objects[objId] is IBeginPlay other) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => other.BeginPlay(), exception => new BeginPlayException(other, exception));
                    #else
                    other.BeginPlay();
                    #endif
                }
            }
        }
        
        public static async Task TryBeginPlayAsync<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IBeginPlayAsync otherAsync) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    await DebugUtility.ReThrowAsync(() => otherAsync.BeginPlay(), exception => new BeginPlayAsyncException(otherAsync, exception));
                    #else
                    await otherAsync.BeginPlay();
                    #endif
                } else if (objects[objId] is IBeginPlay other) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => other.BeginPlay(), exception => new BeginPlayException(other, exception));
                    #else
                    other.BeginPlay();
                    #endif
                }
            }
        }
        
        public static void TryBeginPlay<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IBeginPlayAsync otherAsync) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => otherAsync.BeginPlay(), exception => new BeginPlayAsyncException(otherAsync, exception));
                    #else
                    otherAsync.BeginPlay();
                    #endif
                } else if (objects[objId] is IBeginPlay other) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => other.BeginPlay(), exception => new BeginPlayException(other, exception));
                    #else
                    other.BeginPlay();
                    #endif
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
                    
                    await DebugUtility.ReThrowAsync(() => otherAsync.BeginPlay(), exception => new BeginPlayAsyncException(otherAsync, exception));
                    
                    profile.End();
                    
                    #else
                    await otherAsync.BeginPlay();
                    #endif
                } else if (objects[objId] is IBeginPlay other) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    
                    ProfilerMarker profile = new ProfilerMarker(ProfilerCategory.Scripts, $"{other.GetType().Name}", MarkerFlags.Script);
                    
                    if (other is Object unityObject) {
                        profile.Begin(unityObject);
                    } else {
                        profile.Begin();
                    }
                    
                    DebugUtility.ReThrow(() => other.BeginPlay(), exception => new BeginPlayException(other, exception));
                    
                    profile.End();
                    
                    #else
                    other.BeginPlay();
                    #endif
                }
            }
            
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            group.End();
            #endif
        }
        
        public static void BeginPlay<TBeginPlay>(this TBeginPlay[] objects) where TBeginPlay : IBeginPlay {
            for (int objId = 0; objId < objects.Length; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].BeginPlay(), exception => new BeginPlayException(objects[id], exception));
                #else
                objects[objId].BeginPlay();
                #endif
            }
        }
        
        public static async Task BeginPlayAsync<TBeginPlay>(this TBeginPlay[] objects) where TBeginPlay : IBeginPlayAsync {
            for (int objId = 0; objId < objects.Length; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                await DebugUtility.ReThrowAsync(() => objects[id].BeginPlay(), exception => new BeginPlayAsyncException(objects[id], exception));
                #else
                await objects[objId].BeginPlay();
                #endif
            }
        }
        
        public static void BeginPlay<TBeginPlay>(this List<TBeginPlay> objects) where TBeginPlay : IBeginPlay {
            for (int objId = 0; objId < objects.Count; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].BeginPlay(), exception => new BeginPlayException(objects[id], exception));
                #else
                objects[objId].BeginPlay();
                #endif
            }
        }
        
        public static async Task BeginPlayAsync<TBeginPlay>(this List<TBeginPlay> objects) where TBeginPlay : IBeginPlayAsync {
            for (int objId = 0; objId < objects.Count; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                await DebugUtility.ReThrowAsync(() => objects[id].BeginPlay(), exception => new BeginPlayAsyncException(objects[id], exception));
                #else
                await objects[objId].BeginPlay();
                #endif
            }
        }
    }
}