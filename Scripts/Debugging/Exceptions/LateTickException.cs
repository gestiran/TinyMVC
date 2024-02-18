#if UNITY_EDITOR || DEVELOPMENT_BUILD

    using System;
    using TinyMVC.Loop;

    namespace TinyMVC.Debugging.Exceptions {
        internal sealed class LateTickException : MVCException {
            public readonly ILateTick other;

            public LateTickException(ILateTick other, Exception innerException) : base($"LateTick.Error: {DebugUtility.Link(other)}", innerException) {
                this.other = other;
            }
        }
    }

#endif