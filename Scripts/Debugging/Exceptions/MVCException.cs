#if UNITY_EDITOR || DEVELOPMENT_BUILD

using System;

namespace TinyMVC.Debugging {
    internal abstract class MVCException : Exception {
        protected MVCException(string message, Exception exception) : base(message, exception) { }
    }
}

#endif