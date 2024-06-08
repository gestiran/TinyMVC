using UnityEngine;

namespace TinyMVC.Utilities {
    public static class TimelineUtility {
        public static uint frameId { get; private set; } = 1;
        
        internal static void Next() => frameId++;
        
        #if UNITY_EDITOR
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset() => frameId = 1;
        
        #endif
    }
}