using System.Collections.Generic;
using TinyMVC.Loop;

#if UNITY_EDITOR
using System;
using UnityEngine;
#endif

namespace TinyMVC.Extensions {
    public static class InitExtension {
        public static void TryInit<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IInit other) {
                #if UNITY_EDITOR
                    try {
                    #endif
                        
                        other.Init();
                        
                    #if UNITY_EDITOR
                    } catch (Exception e) {
                        Debug.LogError($"Init.Error: {other.GetType().Name}\n{e}");
                    }
                #endif
                }
            }
        }

        public static void TryInit<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IInit other) {
                #if UNITY_EDITOR
                    try {
                    #endif
                        
                        other.Init();
                        
                    #if UNITY_EDITOR
                    } catch (Exception e) {
                        Debug.LogError($"Init.Error: {other.GetType().Name}\n{e}");
                    }
                #endif
                }
            }
        }

        public static void Init<TInit>(this TInit[] objects) where TInit : IInit {
            for (int objId = 0; objId < objects.Length; objId++) {
            #if UNITY_EDITOR
                try {
                #endif
                    
                    objects[objId].Init();
                    
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"Init.Error: {objects[objId].GetType().Name}\n{e}");
                }
            #endif
            }
        }

        public static void Init<TInit>(this List<TInit> objects) where TInit : IInit {
            for (int objId = 0; objId < objects.Count; objId++) {
            #if UNITY_EDITOR
                try {
                #endif
                    
                    objects[objId].Init();
                    
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"Init.Error: {objects[objId].GetType().Name}\n{e}");
                }
            #endif
            }
        }
    }
}