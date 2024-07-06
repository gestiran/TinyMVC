using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Debugging;

namespace TinyMVC.Loop.Extensions {
    public static class InitExtension {
        public static void TryInit<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IInitAsync otherAsync) {
                    DebugUtility.CheckAndLogException(otherAsync.Init, objects[objId]);
                } else if (objects[objId] is IInit other) {
                    DebugUtility.CheckAndLogException(other.Init, objects[objId]);
                }
            }
        }
        
        public static async Task TryInitAsync<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IInitAsync otherAsync) {
                    await DebugUtility.CheckAndLogExceptionAsync(otherAsync.Init, objects[objId]);
                } else if (objects[objId] is IInit other) {
                    DebugUtility.CheckAndLogException(other.Init, objects[objId]);
                }
            }
        }
        
        public static void TryInit<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IInitAsync otherAsync) {
                    DebugUtility.CheckAndLogException(otherAsync.Init, objects[objId]);
                } else if (objects[objId] is IInit other) {
                    DebugUtility.CheckAndLogException(other.Init, objects[objId]);
                }
            }
        }
        
        public static async Task TryInitAsync<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IInitAsync otherAsync) {
                    await DebugUtility.CheckAndLogExceptionAsync(otherAsync.Init, objects[objId]);
                } else if (objects[objId] is IInit other) {
                    DebugUtility.CheckAndLogException(other.Init, objects[objId]);
                }
            }
        }
        
        public static void Init<TInit>(this TInit[] objects) where TInit : IInit {
            for (int objId = 0; objId < objects.Length; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].Init, objects[objId]);
            }
        }
        
        public static async Task InitAsync<TInit>(this TInit[] objects) where TInit : IInitAsync {
            for (int objId = 0; objId < objects.Length; objId++) {
                await DebugUtility.CheckAndLogExceptionAsync(objects[objId].Init, objects[objId]);
            }
        }
        
        public static void Init<TInit>(this List<TInit> objects) where TInit : IInit {
            for (int objId = 0; objId < objects.Count; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].Init, objects[objId]);
            }
        }
        
        public static async Task InitAsync<TInit>(this List<TInit> objects) where TInit : IInitAsync {
            for (int objId = 0; objId < objects.Count; objId++) {
                await DebugUtility.CheckAndLogExceptionAsync(objects[objId].Init, objects[objId]);
            }
        }
    }
}