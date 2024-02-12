#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;

    namespace TinyMVC.Exceptions {
        public sealed class ModelsException : Exception {
            public ModelsException(Exception innerException) : base("Models.Error", innerException) { }
        }
    }
#endif