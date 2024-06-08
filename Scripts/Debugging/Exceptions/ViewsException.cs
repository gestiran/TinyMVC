#if UNITY_EDITOR || DEVELOPMENT_BUILD

using System;
using TinyMVC.Views;

namespace TinyMVC.Debugging.Exceptions {
    internal sealed class ViewsException : MVCException {
        public ViewsException(IView other, Exception innerException) : base($"View.Error: {DebugUtility.Link(other)}", innerException) { }
    }
}

#endif