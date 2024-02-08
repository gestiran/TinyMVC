using System;
using JetBrains.Annotations;

namespace TinyMVC.Controllers {
    public abstract class Controller : IController {
        private Action<IController> _connectController;
        private Action<IController> _disconnectController;

        internal void ConnectToContext(Action<IController> connectController, Action<IController> disconnectController) {
            _connectController = connectController;
            _disconnectController = disconnectController;
        }

        protected T ConnectController<T>([NotNull] T controller) where T : class, IController {
            if (controller is Controller root) {
                root.ConnectToContext(_connectController, _disconnectController);
            }
            
            _connectController(controller);
            return controller;
        }

        protected void DisconnectController<T>([NotNull] T controller) where T : class, IController => _disconnectController(controller);
    }
}