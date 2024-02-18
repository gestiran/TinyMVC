#if UNITY_EDITOR || DEVELOPMENT_BUILD

    using System;

    namespace TinyMVC.Debugging.Exceptions {
        internal sealed class ModelsException : MVCException {
            public ModelsException(Exception innerException) : base("Models.Error", innerException) { }
        }
    }

#endif