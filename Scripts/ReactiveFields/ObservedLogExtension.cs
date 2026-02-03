using TinyMVC.Boot;
using TinyReactive;
using TinyReactive.Fields;
using UnityEngine;

namespace TinyMVC.ReactiveFields {
    public static class ObservedLogExtension {
        public static void LogChanges<T>(this Observed<T> obj, string label) {
            obj.AddListener(value => Debug.Log($"{label}: {value}"), ProjectContext.scene);
        }
        
        public static void LogChanges<T>(this Observed<T> obj, string label, IUnloadLink unload) {
            obj.AddListener(value => Debug.Log($"{label}: {value}"), unload);
        }
        
        public static void LogWarningChanges<T>(this Observed<T> obj, string label) {
            obj.AddListener(value => Debug.LogWarning($"{label}: {value}"), ProjectContext.scene);
        }
        
        public static void LogWarningChanges<T>(this Observed<T> obj, string label, IUnloadLink unload) {
            obj.AddListener(value => Debug.LogWarning($"{label}: {value}"), unload);
        }
        
        public static void LogErrorChanges<T>(this Observed<T> obj) {
            obj.AddListener(value => Debug.LogError($"Observed: {value}"), ProjectContext.scene);
        }
        
        public static void LogErrorChanges<T>(this Observed<T> obj, string label) {
            obj.AddListener(value => Debug.LogError($"{label}: {value}"), ProjectContext.scene);
        }
        
        public static void LogErrorChanges<T>(this Observed<T> obj, string label, IUnloadLink unload) {
            obj.AddListener(value => Debug.LogError($"{label}: {value}"), unload);
        }
    }
}