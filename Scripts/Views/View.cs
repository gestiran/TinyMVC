using System;
using JetBrains.Annotations;
using UnityEngine;

namespace TinyMVC.Views {
    public abstract class View : MonoBehaviour, IView {
        private Connector _connector;
        
        internal sealed class Connector {
            internal Action<IView> connect;
            internal Action<IView[]> connectArray;
            internal Action<IView> disconnect;
            internal Action<IView[]> disconnectArray;
        }
        
        internal void ApplyConnector(Connector connector) => _connector = connector;

        protected T ConnectView<T>([NotNull] T view) where T : class, IView {
            TryApplyConnector(view);
            _connector.connect(view);
            return view;
        }
        
        protected void ConnectView([NotNull] params IView[] views) {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                TryApplyConnector(views[viewId]);
            }
            
            _connector.connectArray(views);
        }

        protected void DisconnectView<T>([NotNull] T view) where T : class, IView => _connector.disconnect(view);
        
        protected void DisconnectView([NotNull] params IView[] views) => _connector.disconnectArray(views);
        
        private bool TryApplyConnector<T>(T controller) where T : class, IView {
            if (controller is not View root) {
                return false;
            }

            root.ApplyConnector(_connector);
            return true;
        }
    }
}