using System;
using System.Collections.Generic;
using UnityEngine;

namespace TinyMVC.Loop.Extensions {
    public static class LateTickExtension {
        public static void LateTick<T>(this ICollection<T> collection) where T : ILateTick {
            foreach (T obj in collection) {
                try {
                    obj.LateTick();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
    }
}