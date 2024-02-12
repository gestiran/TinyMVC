#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;

    namespace TinyMVC.Exceptions {
        public sealed class BindException : Exception {
            public BindException(Exception innerException) : base("Bind.Error", innerException) { }
        }
    }
#endif