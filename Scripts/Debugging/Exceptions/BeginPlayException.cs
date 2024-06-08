#if UNITY_EDITOR || DEVELOPMENT_BUILD

using System;
using TinyMVC.Loop;

namespace TinyMVC.Debugging.Exceptions {
    internal sealed class BeginPlayException : MVCException {
        public readonly IBeginPlay other;
        
        public BeginPlayException(IBeginPlay other, Exception innerException) : base($"BeginPlay.Error: {DebugUtility.Link(other)}", innerException) {
            this.other = other;
        }
    }
}

#endif