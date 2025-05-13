using System;
using System.Collections.Generic;
using UnityEngine;

namespace TinyMVC.Loop.Extensions {
    public static class FixedTickExtension {
        public static void FixedTick<T>(this ICollection<T> collection) where T : IFixedTick {
            foreach (T obj in collection) {
                try {
                    obj.FixedTick();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
    }
}