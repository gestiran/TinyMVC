using System.Collections.Generic;
using TinyMVC.Loop;

#if UNITY_EDITOR
using System;
using UnityEngine;
#endif

namespace TinyMVC.Extensions {
    public static class BeginPlayExtension {
        public static void TryBeginPlay<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IBeginPlay other) {
                #if UNITY_EDITOR
                    try {
                    #endif
                        
                        other.BeginPlay();
                        
                    #if UNITY_EDITOR
                    } catch (Exception e) {
                        Debug.LogError($"BeginPlay.Error: {other.GetType().Name}\n{e}");
                    }
                #endif
                }
            }
        }
        
        public static void TryBeginPlay<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IBeginPlay other) {
                #if UNITY_EDITOR
                    try {
                    #endif
                        
                        other.BeginPlay();
                        
                    #if UNITY_EDITOR
                    } catch (Exception e) {
                        Debug.LogError($"BeginPlay.Error: {other.GetType().Name}\n{e}");
                    }
                #endif
                }
            }
        }
        
        public static void BeginPlay<TBeginPlay>(this TBeginPlay[] objects) where TBeginPlay : IBeginPlay {
            for (int objId = 0; objId < objects.Length; objId++) {
            #if UNITY_EDITOR
                try {
                #endif
                        
                    objects[objId].BeginPlay();
                        
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"BeginPlay.Error: {objects[objId].GetType().Name}\n{e}");
                }
            #endif
            }
        }
        
        public static void BeginPlay<TBeginPlay>(this List<TBeginPlay> objects) where TBeginPlay : IBeginPlay {
            for (int objId = 0; objId < objects.Count; objId++) {
            #if UNITY_EDITOR
                try {
                #endif
                        
                    objects[objId].BeginPlay();
                        
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"BeginPlay.Error: {objects[objId].GetType().Name}\n{e}");
                }
            #endif
            }
        }
    }
}