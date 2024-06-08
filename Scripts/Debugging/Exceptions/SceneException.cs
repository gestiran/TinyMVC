#if UNITY_EDITOR || DEVELOPMENT_BUILD

using System;

namespace TinyMVC.Debugging.Exceptions {
    internal sealed class SceneException : MVCException {
        public SceneException(string sceneName, Exception innerException) : base($"Scene.Error: {sceneName}", innerException) { }
    }
}

#endif