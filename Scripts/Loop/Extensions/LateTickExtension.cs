using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TinyMVC.Loop.Extensions {
    public static class LateTickExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LateTick<T>(this ICollection<T> collection) where T : ILateTick {
            foreach (T obj in collection) {
                obj.LateTick();
            }
        }
    }
}