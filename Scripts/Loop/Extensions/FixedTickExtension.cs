using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TinyMVC.Loop.Extensions {
    public static class FixedTickExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FixedTick<T>(this ICollection<T> collection) where T : IFixedTick {
            foreach (T obj in collection) {
                obj.FixedTick();
            }
        }
    }
}