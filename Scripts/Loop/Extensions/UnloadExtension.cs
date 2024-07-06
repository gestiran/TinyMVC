using System.Collections.Generic;
using TinyMVC.Debugging;

namespace TinyMVC.Loop.Extensions {
    public static class UnloadExtension {
        public static void TryUnload<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IUnload other) {
                    DebugUtility.CheckAndLogException(other.Unload, objects[objId]);
                }
            }
        }
        
        public static void TryUnload<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IUnload other) {
                    DebugUtility.CheckAndLogException(other.Unload, objects[objId]);
                }
            }
        }
        
        public static void Unload<TUnload>(this TUnload[] objects) where TUnload : IUnload {
            for (int objId = 0; objId < objects.Length; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].Unload, objects[objId]);
            }
        }
        
        public static void Unload<TUnload>(this List<TUnload> objects) where TUnload : IUnload {
            for (int objId = 0; objId < objects.Count; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].Unload, objects[objId]);
            }
        }
        
        public static void Unload<TUnload>(this Dictionary<TUnload, TUnload> objects) where TUnload : IUnload {
            foreach (KeyValuePair<TUnload, TUnload> unload in objects) {
                DebugUtility.CheckAndLogException(unload.Key.Unload, unload.Key);
                DebugUtility.CheckAndLogException(unload.Value.Unload, unload.Value);
            }
        }
        
        public static void UnloadKeys<TUnload, T>(this Dictionary<TUnload, T> objects) where TUnload : IUnload {
            foreach (TUnload unload in objects.Keys) {
                DebugUtility.CheckAndLogException(unload.Unload, unload);
            }
        }
        
        public static void UnloadValues<T, TUnload>(this Dictionary<T, TUnload> objects) where TUnload : IUnload {
            foreach (TUnload unload in objects.Values) {
                DebugUtility.CheckAndLogException(unload.Unload, unload);
            }
        }
    }
}