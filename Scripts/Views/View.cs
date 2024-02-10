using System;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Views {
    [DisallowMultipleComponent]
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
        
        protected void ConnectView([NotNull] IView[] views) {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                TryApplyConnector(views[viewId]);
            }
            
            _connector.connectArray(views);
        }

        protected void DisconnectView<T>([NotNull] T view) where T : class, IView => _connector.disconnect(view);
        
        protected void DisconnectView([NotNull] IView[] views) => _connector.disconnectArray(views);
        
        private bool TryApplyConnector<T>(T controller) where T : class, IView {
            if (controller is not View root) {
                return false;
            }

            root.ApplyConnector(_connector);
            return true;
        }
    }
}