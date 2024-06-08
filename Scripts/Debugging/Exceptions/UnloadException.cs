#if UNITY_EDITOR || DEVELOPMENT_BUILD

using System;
using TinyMVC.Loop;

namespace TinyMVC.Debugging.Exceptions {
    internal sealed class UnloadException : MVCException {
        public readonly IUnload other;
        
        public UnloadException(IUnload other, Exception innerException) : base($"BeginPlay.Error: {DebugUtility.Link(other)}", innerException) {
            this.other = other;
        }
    }
}

#endif