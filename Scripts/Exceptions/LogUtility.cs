#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using UnityEditor;
    using UnityEngine;

    namespace TinyMVC.Exceptions {
        public static class LogUtility {
            public static string Link(object context) {
            #if UNITY_EDITOR
                if (context is MonoBehaviour behavior) {
                    return $"<a href=\"{AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(behavior))}\">{context.GetType().Name}</a>";
                }

                if (context is ScriptableObject scriptable) {
                    return $"<a href=\"{AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(scriptable))}\">{context.GetType().Name}</a>";
                }
                
            #endif
                return context.GetType().Name;
            }
        }
    }
#endif