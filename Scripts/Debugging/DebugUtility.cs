using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace TinyMVC.Debugging {
    internal static class DebugUtility {
        public static void ProfilerMarkerScripts(string name, Action action) {
            ProfilerMarker marker = new ProfilerMarker(ProfilerCategory.Scripts, name);
            marker.Begin();
            action.Invoke();
            marker.End();
        }
        
        public static string Link(object context) {
            if (context == null) {
                return "null";
            }
            
        #if UNITY_EDITOR
            if (TryGetPath(context, out string path)) {
                return $"<a href=\"{path}\">{context.GetType().Name}</a>";
            }
            
        #endif
            return context.GetType().Name;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogException(Exception exception, object context) {
            if (context != null && context is UnityObject obj) {
                Debug.LogError(exception.StackTrace, obj);
            } else {
                LogException(exception);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogException(Exception exception) => Debug.LogError(exception.StackTrace);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckAndLogException(Action action) {
        #if MVC_DEBUG
            try {
        #endif
            
            action.Invoke();
            
        #if MVC_DEBUG
            } catch (Exception exception) {
                LogException(exception);
            }
        #endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CheckAndLogExceptionResult<T>(Func<T> func) {
        #if MVC_DEBUG
            try {
        #endif
            
            return func.Invoke();
            
        #if MVC_DEBUG
            } catch (Exception exception) {
                LogException(exception);
                return default;
            }
        #endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckAndLogException(Func<Task> func) {
        #if MVC_DEBUG
            try {
        #endif
            
            func.Invoke();
            
        #if MVC_DEBUG
            } catch (Exception exception) {
                LogException(exception);
            }
        #endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckAndLogException(Func<Task> func, object context) {
        #if MVC_DEBUG
            try {
        #endif
            
            func.Invoke();
            
        #if MVC_DEBUG
            } catch (Exception exception) {
                LogException(exception, context);
            }
        #endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task CheckAndLogExceptionAsync(Func<Task> func) {
        #if MVC_DEBUG
            try {
        #endif
            
            await func.Invoke();
            
        #if MVC_DEBUG
            } catch (Exception exception) {
                LogException(exception);
            }
        #endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task CheckAndLogExceptionAsync(Func<Task> func, object context) {
        #if MVC_DEBUG
            try {
        #endif
            
            await func.Invoke();
            
        #if MVC_DEBUG
            } catch (Exception exception) {
                LogException(exception, context);
            }
        #endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckAndLogException<T>(Action<T> action, T value) {
        #if MVC_DEBUG
            try {
        #endif
            
            action.Invoke(value);
            
        #if MVC_DEBUG
            } catch (Exception exception) {
                LogException(exception);
            }
        #endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckAndLogException<T1, T2>(Action<T1, T2> action, T1 value1, T2 value2) {
        #if MVC_DEBUG
            try {
        #endif
            
            action.Invoke(value1, value2);
            
        #if MVC_DEBUG
            } catch (Exception exception) {
                LogException(exception);
            }
        #endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckAndLogException<T1, T2, T3>(Action<T1, T2, T3> action, T1 value1, T2 value2, T3 value3) {
        #if MVC_DEBUG
            try {
        #endif
            
            action.Invoke(value1, value2, value3);
            
        #if MVC_DEBUG
            } catch (Exception exception) {
                LogException(exception);
            }
        #endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckAndLogException(Action action, object context) {
        #if MVC_DEBUG
            try {
        #endif
            
            action.Invoke();
            
        #if MVC_DEBUG
            } catch (Exception exception) {
                LogException(exception, context);
            }
        #endif
        }
        
        public static string Link(string contextName) {
        #if UNITY_EDITOR
            if (TryGetPath(contextName, out string path)) {
                return $"<a href=\"{path}\">{contextName}</a>";
            }
            
        #endif
            return contextName;
        }
        
    #if UNITY_EDITOR
        
        private static bool TryGetPath(object context, out string path) {
            if (context is MonoBehaviour behavior) {
                path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromMonoBehaviour(behavior));
                
                return true;
            }
            
            if (context is ScriptableObject scriptable) {
                path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromScriptableObject(scriptable));
                
                return true;
            }
            
            return TryGetPath(context.GetType().Name, out path);
        }
        
        private static bool TryGetPath(string contextName, out string path) {
            string[] scripts = UnityEditor.AssetDatabase.FindAssets($"t:Script {contextName}");
            
            if (scripts.Length > 0) {
                path = UnityEditor.AssetDatabase.GUIDToAssetPath(scripts[0]);
                
                return true;
            }
            
            path = "";
            
            return false;
        }
        
    #endif
    }
}