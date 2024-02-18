#if UNITY_EDITOR || DEVELOPMENT_BUILD
    namespace TinyMVC.Debugging {
        internal static class DebugExtension {
            public static string Bold(this string str) => $"<b>{str}</b>";
        }
    }
#endif