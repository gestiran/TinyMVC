using System.Collections.Generic;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using TinyMVC.Debugging;
using TinyMVC.Debugging.Exceptions;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class UnloadExtension {
        public static void TryUnload<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IUnload other) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => other.Unload(), exception => new UnloadException(other, exception));
                #else
                    other.Unload();
                #endif
                }
            }
        }
        
        public static void TryUnload<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IUnload other) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => other.Unload(), exception => new UnloadException(other, exception));
                #else
                    other.Unload();
                #endif
                }
            }
        }
        
        public static void Unload<TUnload>(this TUnload[] objects) where TUnload : IUnload {
            for (int objId = 0; objId < objects.Length; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].Unload(), exception => new UnloadException(objects[id], exception));
            #else
                objects[objId].Unload();
            #endif
            }
        }
        
        public static void Unload<TUnload>(this List<TUnload> objects) where TUnload : IUnload {
            for (int objId = 0; objId < objects.Count; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].Unload(), exception => new UnloadException(objects[id], exception));
            #else
                objects[objId].Unload();
            #endif
            }
        }
        
        public static void Unload<TUnload>(this Dictionary<TUnload, TUnload> objects) where TUnload : IUnload {
            foreach (KeyValuePair<TUnload, TUnload> unload in objects) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif
                    
                    unload.Key.Unload();
                    unload.Value.Unload();
                    
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new UnloadException(unload.Key, exception);
                }
            #endif
            }
        }
        
        public static void UnloadKeys<TUnload, T>(this Dictionary<TUnload, T> objects) where TUnload : IUnload {
            foreach (TUnload unload in objects.Keys) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugUtility.ReThrow(() => unload.Unload(), exception => new UnloadException(unload, exception));
            #else
                unload.Unload();
            #endif
            }
        }
        
        public static void UnloadValues<T, TUnload>(this Dictionary<T, TUnload> objects) where TUnload : IUnload {
            foreach (TUnload unload in objects.Values) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugUtility.ReThrow(() => unload.Unload(), exception => new UnloadException(unload, exception));
            #else
                unload.Unload();
            #endif
            }
        }
    }
}