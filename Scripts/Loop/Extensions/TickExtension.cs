using System;
using System.Collections.Generic;
using UnityEngine;

namespace TinyMVC.Loop.Extensions {
    public static class TickExtension {
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