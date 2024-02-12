#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;
    using TinyMVC.Views;

    namespace TinyMVC.Exceptions {
        public sealed class ViewsException : Exception {
            public ViewsException(IView other, Exception innerException) : base($"View.Error: {LogUtility.Link(other)}", innerException) { }
        }
    }
#endif