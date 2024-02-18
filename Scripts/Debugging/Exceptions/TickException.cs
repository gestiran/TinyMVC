#if UNITY_EDITOR || DEVELOPMENT_BUILD

    using System;
    using TinyMVC.Loop;

    namespace TinyMVC.Debugging.Exceptions {
        internal sealed class TickException : MVCException {
            public readonly ITick other;

            public TickException(ITick other, Exception innerException) : base($"Tick.Error: {DebugUtility.Link(other)}", innerException) => this.other = other;
        }
    }

#endif