using System.Collections.Generic;
using TinyMVC.Loop;

namespace TinyMVC.Extensions {
    public static class UnloadExtension {
        public static void TryUnload<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IUnload other) {
                    other.Unload();
                }
            }
        }
        
        public static void TryUnload<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IUnload other) {
                    other.Unload();
                }
            }
        }
        
        public static void Unload<TUnload>(this TUnload[] objects) where TUnload : IUnload {
            for (int objId = 0; objId < objects.Length; objId++) {
                objects[objId].Unload();
            }
        }
        
        public static void Unload<TUnload>(this List<TUnload> objects) where TUnload : IUnload {
            for (int objId = 0; objId < objects.Count; objId++) {
                objects[objId].Unload();
            }
        }
        
        public static void Unload<TUnload>(this Dictionary<TUnload, TUnload> objects) where TUnload : IUnload {
            foreach (KeyValuePair<TUnload, TUnload> unload in objects) {
                unload.Key.Unload();
                unload.Value.Unload();
            }
        }
        
        public static void UnloadKeys<TUnload, T>(this Dictionary<TUnload, T> objects) where TUnload : IUnload {
            foreach (TUnload unload in objects.Keys) {
                unload.Unload();
            }
        }
        
        public static void UnloadValues<T, TUnload>(this Dictionary<T, TUnload> objects) where TUnload : IUnload {
            foreach (TUnload unload in objects.Values) {
                unload.Unload();
            }
        }
    }
}