using TinyMVC.Boot;
using TinyReactive;
using TinyReactive.Fields;
using TinyUtilities.Logger;

namespace TinyMVC.ReactiveFields {
    /// <summary>
    /// Extensions for monitoring changes to <see cref="TinyReactive.Fields.Observed{T}">Observed</see> fields
    /// and outputting them to the <see cref="UnityEngine.Debug">Debug</see> log.
    /// </summary>
    public static class ObservedLogExtension {
        /// <summary> Logs a change message to the Unity Log Console. </summary>
        /// <param name="obj"> Current Observed field </param>
        /// <param name="label"> Prefix or title. </param>
        /// <typeparam name="T"> Observed field type. </typeparam>
        public static Observed<T> LogChanges<T>(this Observed<T> obj, string label) {
            return obj.AddListener(value => DebugUtility.Log($"{label}: {value}"), ProjectContext.scene);
        }
        
        /// <summary> Logs a change message to the Unity Log Console with unload link. </summary>
        /// <param name="obj"> Current Observed field </param>
        /// <param name="label"> Prefix or title. </param>
        /// <param name="unload"> Link to auto unload trigger. </param>
        /// <typeparam name="T"> Observed field type. </typeparam>
        public static Observed<T> LogChanges<T>(this Observed<T> obj, string label, IUnloadLink unload) {
            return obj.AddListener(value => DebugUtility.Log($"{label}: {value}"), unload);
        }
        
        /// <summary> Logs a change message to the Unity Warning Console. </summary>
        /// <param name="obj"> Current Observed field </param>
        /// <param name="label"> Prefix or title. </param>
        /// <typeparam name="T"> Observed field type. </typeparam>
        public static Observed<T> LogWarningChanges<T>(this Observed<T> obj, string label) {
            return obj.AddListener(value => DebugUtility.LogWarning($"{label}: {value}"), ProjectContext.scene);
        }
        
        /// <summary> Logs a change message to the Unity Warning Console with unload link. </summary>
        /// <param name="obj"> Current Observed field </param>
        /// <param name="label"> Prefix or title. </param>
        /// <param name="unload"> Link to auto unload trigger. </param>
        /// <typeparam name="T"> Observed field type. </typeparam>
        public static Observed<T> LogWarningChanges<T>(this Observed<T> obj, string label, IUnloadLink unload) {
            return obj.AddListener(value => DebugUtility.LogWarning($"{label}: {value}"), unload);
        }
        
        /// <summary> Logs a change message to the Unity Error Console. </summary>
        /// <param name="obj"> Current Observed field </param>
        /// <typeparam name="T"> Observed field type. </typeparam>
        public static Observed<T> LogErrorChanges<T>(this Observed<T> obj) {
            return obj.AddListener(value => DebugUtility.LogError($"Observed: {value}"), ProjectContext.scene);
        }
        
        /// <summary> Logs a change message to the Unity Error Console. </summary>
        /// <param name="obj"> Current Observed field </param>
        /// <param name="label"> Prefix or title. </param>
        /// <typeparam name="T"> Observed field type. </typeparam>
        public static Observed<T> LogErrorChanges<T>(this Observed<T> obj, string label) {
            return obj.AddListener(value => DebugUtility.LogError($"{label}: {value}"), ProjectContext.scene);
        }
        
        /// <summary> Logs a change message to the Unity Error Console with unload link. </summary>
        /// <param name="obj"> Current Observed field </param>
        /// <param name="label"> Prefix or title. </param>
        /// <param name="unload"> Link to auto unload trigger. </param>
        /// <typeparam name="T"> Observed field type. </typeparam>
        public static Observed<T> LogErrorChanges<T>(this Observed<T> obj, string label, IUnloadLink unload) {
            return obj.AddListener(value => DebugUtility.LogError($"{label}: {value}"), unload);
        }
    }
}