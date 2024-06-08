using System.Collections.Generic;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TinyMVC.Debugging;
using TinyMVC.Debugging.Exceptions;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class LateTickExtension {
        public static void LateTick<T>(this T[] objects) where T : ILateTick {
            for (int objId = 0; objId < objects.Length; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].LateTick(), exception => new LateTickException(objects[id], exception));
                #else
                objects[objId].LateTick();
                #endif
            }
        }
        
        public static void LateTick<T>(this List<T> objects) where T : ILateTick {
            for (int objId = 0; objId < objects.Count; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].LateTick(), exception => new LateTickException(objects[id], exception));
                #else
                objects[objId].LateTick();
                #endif
            }
        }
    }
}