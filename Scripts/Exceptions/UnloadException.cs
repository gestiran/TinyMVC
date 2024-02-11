#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;
    using TinyMVC.Loop;

    namespace TinyMVC.Exceptions {
        public sealed class UnloadException : Exception {
            public readonly IUnload other;

            public UnloadException(IUnload other, Exception innerException) : base($"BeginPlay.Error: {other.GetType().Name}", innerException) {
                this.other = other;
            }
        }
    }
#endif