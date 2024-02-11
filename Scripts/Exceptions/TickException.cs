#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;
    using TinyMVC.Loop;

    namespace TinyMVC.Exceptions {
        public sealed class TickException : Exception {
            public readonly ITick other;

            public TickException(ITick other, Exception innerException) : base($"Tick.Error: {other.GetType().Name}", innerException) { this.other = other; }
        }
    }
#endif