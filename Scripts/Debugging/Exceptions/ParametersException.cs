#if UNITY_EDITOR || DEVELOPMENT_BUILD

using System;

namespace TinyMVC.Debugging.Exceptions {
    internal sealed class ParametersException : MVCException {
        public ParametersException(Exception innerException) : base("Parameters.Error", innerException) { }
    }
}

#endif