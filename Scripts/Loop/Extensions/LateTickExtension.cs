using System.Collections.Generic;
using TinyMVC.Debugging;

namespace TinyMVC.Loop.Extensions {
    public static class LateTickExtension {
        public static void LateTick<T>(this T[] objects) where T : ILateTick {
            for (int objId = 0; objId < objects.Length; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].LateTick, objects[objId]);
            }
        }
        
        public static void LateTick<T>(this List<T> objects) where T : ILateTick {
            for (int objId = 0; objId < objects.Count; objId++) {
                DebugUtility.CheckAndLogException(objects[objId].LateTick, objects[objId]);
            }
        }
    }
}