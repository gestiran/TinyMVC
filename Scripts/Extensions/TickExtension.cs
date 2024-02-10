using System.Collections.Generic;
using TinyMVC.Loop;

#if UNITY_EDITOR
using System;
using UnityEngine;
#endif

namespace TinyMVC.Extensions {
    public static class TickExtension {
        public static void Tick<T>(this T[] objects) where T : ITick {
            for (int objId = 0; objId < objects.Length; objId++) {
            #if UNITY_EDITOR
                try {
                #endif
                        
                    objects[objId].Tick();
                        
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"Tick.Error: {objects[objId].GetType().Name}\n{e}");
                }
            #endif
            }
        }

        public static void Tick<T>(this List<T> objects) where T : ITick {
            for (int objId = 0; objId < objects.Count; objId++) {
            #if UNITY_EDITOR
                try {
                #endif
                        
                    objects[objId].Tick();
                        
                #if UNITY_EDITOR
                } catch (Exception e) {
                    Debug.LogError($"Tick.Error: {objects[objId].GetType().Name}\n{e}");
                }
            #endif
            }
        }
    }
}