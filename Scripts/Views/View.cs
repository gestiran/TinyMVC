using System;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Views {
    [DisallowMultipleComponent]
    public abstract class View : MonoBehaviour, IView {
        private Action<IView> _connectView;
        private Action<IView> _disconnectView;
        
        internal void ConnectToContext(Action<IView> connectView, Action<IView> disconnectController) {
            _connectView = connectView;
            _disconnectView = disconnectController;
        }

        protected T ConnectView<T>([NotNull] T view) where T : class, IView {
            if (view is View root) {
                root.ConnectToContext(_connectView, _disconnectView);
            }
            
            _connectView(view);
            return view;
        }

        protected void DisconnectView<T>([NotNull] T view) where T : class, IView => _disconnectView(view);
        
    #if UNITY_EDITOR && ODIN_INSPECTOR
        
        [Button("Generate", DirtyOnClick = true), PropertyOrder(1000)]
        public virtual void Generate_Editor() { }
        
    #endif
    }
}