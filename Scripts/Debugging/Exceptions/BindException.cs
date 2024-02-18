#if UNITY_EDITOR || DEVELOPMENT_BUILD

    using System;

    namespace TinyMVC.Debugging.Exceptions {
        internal sealed class BindException : MVCException {
            public BindException(Exception innerException) : base("Bind.Error", innerException) { }
        }
    }

#endif