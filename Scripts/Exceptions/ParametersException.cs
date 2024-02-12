#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;

    namespace TinyMVC.Exceptions {
        public sealed class ParametersException : Exception {
            public ParametersException(Exception innerException) : base("Parameters.Error", innerException) { }
        }
    }
#endif