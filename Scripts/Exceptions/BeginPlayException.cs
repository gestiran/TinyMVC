#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;
    using TinyMVC.Loop;
    
    namespace TinyMVC.Exceptions {
        public sealed class BeginPlayException : Exception {
            public readonly IBeginPlay other;

            public BeginPlayException(IBeginPlay other, Exception innerException) : 
                base($"BeginPlay.Error: {LogUtility.Link(other)}", innerException) {
                this.other = other;
            }
        }
    }
    
#endif