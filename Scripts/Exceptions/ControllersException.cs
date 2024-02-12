#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;
    using TinyMVC.Controllers;

    namespace TinyMVC.Exceptions {
        public sealed class ControllersException : Exception {
            public ControllersException(IController other, Exception innerException) : base($"Controller.Error: {LogUtility.Link(other)}", innerException) { }
        }
    }
#endif