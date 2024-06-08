#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using TinyMVC.Loop;

namespace TinyMVC.Debugging.Exceptions {
    internal sealed class InitAsyncException : MVCException {
        public readonly IInitAsync other;
        
        public InitAsyncException(IInitAsync other, Exception innerException) : base($"InitAsync.Error: {DebugUtility.Link(other)}", innerException) => this.other = other;
    }
}
#endif