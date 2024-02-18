using System.Collections.Generic;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TinyMVC.Debugging;
using TinyMVC.Debugging.Exceptions;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class FixedTickExtension {
        public static void FixedTick<T>(this T[] objects) where T : IFixedTick {
            for (int objId = 0; objId < objects.Length; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].FixedTick(), exception => new FixedTickException(objects[id], exception));
            #else
                objects[objId].FixedTick();
            #endif
            }
        }

        public static void FixedTick<T>(this List<T> objects) where T : IFixedTick {
            for (int objId = 0; objId < objects.Count; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].FixedTick(), exception => new FixedTickException(objects[id], exception));
            #else
                objects[objId].FixedTick();
            #endif
            }
        }
    }
}