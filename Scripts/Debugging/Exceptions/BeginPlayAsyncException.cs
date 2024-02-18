#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;
    using TinyMVC.Loop;

    namespace TinyMVC.Debugging.Exceptions {
    internal sealed class BeginPlayAsyncException : MVCException {
        public readonly IBeginPlayAsync other;

        public BeginPlayAsyncException(IBeginPlayAsync other, Exception innerException) : base($"BeginPlayAsync.Error: {DebugUtility.Link(other)}", innerException) {
            this.other = other;
        }
    }
}
#endif