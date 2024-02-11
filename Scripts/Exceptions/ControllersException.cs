#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;
    using TinyMVC.Controllers;

    namespace TinyMVC.Exceptions {
        public sealed class ControllersException : Exception {
            public readonly IController other;

            public ControllersException(IController other, Exception innerException) : base($"Controller.Error: {other.GetType().Name}", innerException) {
                this.other = other;
            }
        }
    }
#endif