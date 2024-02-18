#if UNITY_EDITOR || DEVELOPMENT_BUILD

    using System;
    using TinyMVC.Controllers;

    namespace TinyMVC.Debugging.Exceptions {
        internal sealed class ControllersException : MVCException {
            public ControllersException(IController other, Exception innerException) : base($"Controller.Error: {DebugUtility.Link(other)}", innerException) { }
        }
    }

#endif