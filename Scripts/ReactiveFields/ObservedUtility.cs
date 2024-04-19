using UnityEngine;

namespace TinyMVC.ReactiveFields {
    internal static class ObservedUtility {
        public static uint frameId { get; private set; } = 1;
        
        public static void Next() => frameId++;

        [RuntimeInitializeOnLoadMethod]
        public static void Reset() => frameId = 1;
    }
}