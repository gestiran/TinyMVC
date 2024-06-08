#if UNITY_EDITOR || DEVELOPMENT_BUILD

using System;
using TinyMVC.Loop;

namespace TinyMVC.Debugging.Exceptions {
    internal sealed class InitException : MVCException {
        public readonly IInit other;
        
        public InitException(IInit other, Exception innerException) : base($"Init.Error: {DebugUtility.Link(other)}", innerException) => this.other = other;
    }
}

#endif