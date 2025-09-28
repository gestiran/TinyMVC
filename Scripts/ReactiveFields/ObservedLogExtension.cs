using TinyMVC.Boot;
using TinyReactive;
using TinyReactive.Fields;
using UnityEngine;

namespace TinyMVC.ReactiveFields {
    public static class ObservedLogExtension {
        public static void LogChanges<T>(this Observed<T> obj, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.Log($"{label}: {value}"), unload);
        }
        
        public static void LogChanges<T>(this Observed<T> obj, string label, UnloadPool unload) {
            obj.AddListener(value => Debug.Log($"{label}: {value}"), unload);
        }
        
        public static void LogWarningChanges<T>(this Observed<T> obj, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.LogWarning($"{label}: {value}"), unload);
        }
        
        public static void LogWarningChanges<T>(this Observed<T> obj, string label, UnloadPool unload) {
            obj.AddListener(value => Debug.LogWarning($"{label}: {value}"), unload);
        }
        
        public static void LogErrorChanges<T>(this Observed<T> obj) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.LogError($"Observed: {value}"), unload);
        }
        
        public static void LogErrorChanges<T>(this Observed<T> obj, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.LogError($"{label}: {value}"), unload);
        }
        
        public static void LogErrorChanges<T>(this Observed<T> obj, string label, UnloadPool unload) {
            obj.AddListener(value => Debug.LogError($"{label}: {value}"), unload);
        }
    }
}