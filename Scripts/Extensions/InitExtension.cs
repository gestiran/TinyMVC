using System.Collections.Generic;
using TinyMVC.Loop;

namespace TinyMVC.Extensions {
    public static class InitExtension {
        public static void TryInit<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IInit other) {
                    other.Init();
                }
            }
        }
        
        public static void TryInit<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IInit other) {
                    other.Init();
                }
            }
        }
        
        public static void Init<TInit>(this TInit[] objects) where TInit : IInit {
            for (int objId = 0; objId < objects.Length; objId++) {
                objects[objId].Init();
            }
        }
        
        public static void Init<TInit>(this List<TInit> objects) where TInit : IInit {
            for (int objId = 0; objId < objects.Count; objId++) {
                objects[objId].Init();
            }
        }
    }
}