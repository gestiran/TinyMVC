using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TinyMVC.Loop.Extensions {
    public static class TickExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Tick<T>(this ICollection<T> collection) where T : ITick {
            foreach (T obj in collection) {
                try {
                    obj.Tick();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
    }
}