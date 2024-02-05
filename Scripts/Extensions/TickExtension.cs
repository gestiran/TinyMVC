using System.Collections.Generic;
using TinyMVC.Loop;

namespace TinyMVC.Extensions {
    public static class TickExtension {
        public static void Tick<T>(this T[] objects) where T : ITick {
            for (int objId = 0; objId < objects.Length; objId++) {
                objects[objId].Tick();
            }
        }

        public static void Tick<T>(this List<T> objects) where T : ITick {
            for (int objId = 0; objId < objects.Count; objId++) {
                objects[objId].Tick();
            }
        }
    }
}