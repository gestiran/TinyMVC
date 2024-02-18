#if UNITY_EDITOR
using UnityEngine;

namespace TinyMVC.ReactiveFields {
    internal static class ObservedTestUtility {
        public static uint frameId { get; private set; } = 1;
        
        public static void Next() => frameId++;

        [RuntimeInitializeOnLoadMethod]
        public static void Reset() => frameId = 1;
    }
}
#endif