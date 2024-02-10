﻿using System;
using JetBrains.Annotations;

namespace TinyMVC.Controllers {
    public abstract class Controller : IController {
        private Connector _connector;

        internal sealed class Connector {
            internal Action<IController> connect;
            internal Action<IController[]> connectArray;
            internal Action<IController> disconnect;
            internal Action<IController[]> disconnectArray;
        }
        
        internal void ApplyConnector(Connector connector) => _connector = connector;

        protected T ConnectController<T>() where T : class, IController, new() {
            T controller = new T();
            TryApplyConnector(controller);
            _connector.connect(controller);
            return controller;
        }
        
        protected T ConnectController<T>([NotNull] T controller) where T : class, IController {
            TryApplyConnector(controller);
            _connector.connect(controller);
            return controller;
        }
        
        protected void ConnectController([NotNull] params IController[] controllers) {
            for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                TryApplyConnector(controllers[controllerId]);
            }
            
            _connector.connectArray(controllers);
        }

        protected void DisconnectController<T>([NotNull] T controller) where T : class, IController => _connector.disconnect(controller);
        
        protected void DisconnectController([NotNull] params IController[] controllers) => _connector.disconnectArray(controllers);

        private bool TryApplyConnector<T>(T controller) where T : class, IController {
            if (controller is not Controller root) {
                return false;
            }

            root.ApplyConnector(_connector);
            return true;
        }
    }
}