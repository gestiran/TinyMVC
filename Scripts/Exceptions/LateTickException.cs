#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;
    using TinyMVC.Loop;

    namespace TinyMVC.Exceptions {
        public sealed class LateTickException : Exception {
            public readonly ILateTick other;

            public LateTickException(ILateTick other, Exception innerException) : base($"LateTick.Error: {LogUtility.Link(other)}", innerException) {
                this.other = other;
            }
        }
    }
#endif