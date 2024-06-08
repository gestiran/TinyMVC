using System.Collections.Generic;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TinyMVC.Debugging;
using TinyMVC.Debugging.Exceptions;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class TickExtension {
        public static void Tick<T>(this T[] objects) where T : ITick {
            for (int objId = 0; objId < objects.Length; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].Tick(), exception => new TickException(objects[id], exception));
                #else
                objects[objId].Tick();
                #endif
            }
        }
        
        public static void Tick<T>(this List<T> objects) where T : ITick {
            for (int objId = 0; objId < objects.Count; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].Tick(), exception => new TickException(objects[id], exception));
                #else
                objects[objId].Tick();
                #endif
            }
        }
    }
}