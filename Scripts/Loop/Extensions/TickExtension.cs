using System.Collections.Generic;
using TinyMVC.Debugging;

namespace TinyMVC.Loop.Extensions {
    public static class TickExtension {
        public static void Tick<T>(this T[] objects) where T : ITick {
            for (int objId = 0; objId < objects.Length; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].Tick, objects[objId]);
            }
        }
        
        public static void Tick<T>(this List<T> objects) where T : ITick {
            for (int objId = 0; objId < objects.Count; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].Tick, objects[objId]);
            }
        }
    }
}