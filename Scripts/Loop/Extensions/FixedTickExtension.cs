using System.Collections.Generic;
using TinyMVC.Debugging;

namespace TinyMVC.Loop.Extensions {
    public static class FixedTickExtension {
        public static void FixedTick<T>(this T[] objects) where T : IFixedTick {
            for (int objId = 0; objId < objects.Length; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].FixedTick, objects[objId]);
            }
        }
        
        public static void FixedTick<T>(this List<T> objects) where T : IFixedTick {
            for (int objId = 0; objId < objects.Count; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].FixedTick, objects[objId]);
            }
        }
    }
}