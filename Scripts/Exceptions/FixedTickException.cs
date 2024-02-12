#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;
    using TinyMVC.Loop;

    namespace TinyMVC.Exceptions {
        public sealed class FixedTickException : Exception {
            public readonly IFixedTick other;

            public FixedTickException(IFixedTick other, Exception innerException) : base($"FixedTick.Error: {LogUtility.Link(other)}", innerException) {
                this.other = other;
            }
        }
    }
#endif