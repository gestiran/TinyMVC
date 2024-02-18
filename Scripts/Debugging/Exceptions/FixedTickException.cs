#if UNITY_EDITOR || DEVELOPMENT_BUILD

    using System;
    using TinyMVC.Loop;

    namespace TinyMVC.Debugging.Exceptions {
        internal sealed class FixedTickException : MVCException {
            public readonly IFixedTick other;

            public FixedTickException(IFixedTick other, Exception innerException) : base($"FixedTick.Error: {DebugUtility.Link(other)}", innerException) {
                this.other = other;
            }
        }
    }

#endif