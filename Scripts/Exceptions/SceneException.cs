#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;

    namespace TinyMVC.Exceptions {
        public sealed class SceneException : Exception {
            public SceneException(string sceneName, Exception innerException) : base($"Scene.Error: {sceneName}", innerException) { }
        }
    }
#endif