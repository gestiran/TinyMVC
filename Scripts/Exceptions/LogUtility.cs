#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using UnityEditor;
    using UnityEngine;

    namespace TinyMVC.Exceptions {
        public static class LogUtility {
            public static string Link(object context) {
            #if UNITY_EDITOR
                if (TryGetPath(context, out string path)) {
                    return $"<a href=\"{path}\">{context.GetType().Name}</a>";
                }

            #endif
                return context.GetType().Name;
            }

            private static bool TryGetPath(object context, out string path) {
                if (context is MonoBehaviour behavior) {
                    path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(behavior));
                    return true;
                }

                if (context is ScriptableObject scriptable) {
                    path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(scriptable));
                    return true;
                }

                string[] scripts = AssetDatabase.FindAssets($"t:Script {context.GetType().Name}");

                if (scripts.Length > 0) {
                    path = AssetDatabase.GUIDToAssetPath(scripts[0]);
                    return true;
                }

                path = "";
                return false;
            }
        }
    }
#endif