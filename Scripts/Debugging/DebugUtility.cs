#if UNITY_EDITOR || DEVELOPMENT_BUILD

    using System;
    using System.Threading.Tasks;
    using Unity.Profiling;
    using UnityEditor;
    using UnityEngine;

    namespace TinyMVC.Debugging {
        internal static class DebugUtility {
            public static void ProfilerMarkerScripts(string name, Action action) {
                ProfilerMarker marker = new ProfilerMarker(ProfilerCategory.Scripts, name);
                marker.Begin();
                action.Invoke();
                marker.End();
            }
            
            public static void ProfilerMarkerScripts<T>(string name, Action action, Func<Exception, T> createException) where T : MVCException {
                ProfilerMarker marker = new ProfilerMarker(ProfilerCategory.Scripts, name);
                marker.Begin();
                
                try {
                    action.Invoke();
                } catch (Exception exception) {
                    throw createException(exception);
                }
                
                marker.End();
            }

            public static void ReThrow<T>(Action action, Func<Exception, T> createException) where T : MVCException {
                try {
                    action.Invoke();
                } catch (Exception exception) {
                    throw createException(exception);
                }
            }
            
            public static async Task ReThrowAsync<T>(Func<Task> action, Func<Exception, T> createException) where T : MVCException {
                try {
                    await action.Invoke();
                } catch (Exception exception) {
                    throw createException(exception);
                }
            }
            
            public static void ReThrow<T1, T2>(Action action, Func<T2, T1> createException) where T1 : MVCException where T2 : Exception {
                try {
                    action.Invoke();
                } catch (T2 exception) {
                    throw createException(exception);
                }
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
                    path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(behavior));

                    return true;
                }

                if (context is ScriptableObject scriptable) {
                    path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(scriptable));

                    return true;
                }
                
                return TryGetPath(context.GetType().Name, out path);
            }
            
            private static bool TryGetPath(string contextName, out string path) {
                string[] scripts = AssetDatabase.FindAssets($"t:Script {contextName}");

                if (scripts.Length > 0) {
                    path = AssetDatabase.GUIDToAssetPath(scripts[0]);

                    return true;
                }

                path = "";

                return false;
            }

        #endif
        }
    }
#endif