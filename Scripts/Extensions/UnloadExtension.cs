using System.Collections.Generic;
using TinyMVC.Loop;

#if UNITY_EDITOR
using System;
using UnityEngine;
#endif

namespace TinyMVC.Extensions {
    public static class UnloadExtension {
        public static void TryUnload<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IUnload other) {
                #if UNITY_EDITOR
                    try {
                    #endif
                        
                        other.Unload();
                        
                    #if UNITY_EDITOR
                    } catch (Exception e) {
                        Debug.LogError($"Unload.Error: {other.GetType().Name}\n{e}");
                    }
                #endif
                }
            }
        }
        
        public static void TryUnload<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IUnload other) {
                #if UNITY_EDITOR
                    try {
                    #endif
                        
                        other.Unload();
                        
                    #if UNITY_EDITOR
                    } catch (Exception e) {
                        Debug.LogError($"Unload.Error: {other.GetType().Name}\n{e}");
                    }
                #endif
                }
            }
        }
        
        public static void Unload<TUnload>(this TUnload[] objects) where TUnload : IUnload {
            for (int objId = 0; objId < objects.Length; objId++) {
            #if UNITY_EDITOR
                try {
                #endif
                        
                    objects[objId].Unload();
                        
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"Unload.Error: {objects[objId].GetType().Name}\n{e}");
                }
            #endif
            }
        }
        
        public static void Unload<TUnload>(this List<TUnload> objects) where TUnload : IUnload {
            for (int objId = 0; objId < objects.Count; objId++) {
            #if UNITY_EDITOR
                try {
                #endif
                        
                    objects[objId].Unload();
                        
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"Unload.Error: {objects[objId].GetType().Name}\n{e}");
                }
            #endif
            }
        }
        
        public static void Unload<TUnload>(this Dictionary<TUnload, TUnload> objects) where TUnload : IUnload {
            foreach (KeyValuePair<TUnload, TUnload> unload in objects) {
            #if UNITY_EDITOR
                try {
                #endif
                    
                    unload.Key.Unload();
                    unload.Value.Unload();
                    
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"Unload.Error: {typeof(TUnload).Name}\n{e}");
                }
            #endif
            }
        }
        
        public static void UnloadKeys<TUnload, T>(this Dictionary<TUnload, T> objects) where TUnload : IUnload {
            foreach (TUnload unload in objects.Keys) {
            #if UNITY_EDITOR
                try {
                #endif
                        
                    unload.Unload();
                        
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"Unload.Error: {unload.GetType().Name}\n{e}");
                }
            #endif
            }
        }
        
        public static void UnloadValues<T, TUnload>(this Dictionary<T, TUnload> objects) where TUnload : IUnload {
            foreach (TUnload unload in objects.Values) {
            #if UNITY_EDITOR
                try {
                #endif
                        
                    unload.Unload();
                        
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"Unload.Error: {unload.GetType().Name}\n{e}");
                }
            #endif
            }
        }
    }
}