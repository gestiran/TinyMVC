using System;
using JetBrains.Annotations;
using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using UnityEngine;

namespace TinyMVC.Views {
    public abstract class View : MonoBehaviour, IView, IViewConnector {
        public ConnectState connectState { get; private set; }

        private Connector _connector;

        public enum ConnectState : byte {
            None,
            Connected,
            Disconnected
        }
        
        internal sealed class Connector {
            internal Action<IView> connect;
            internal Action<IView, Action> connectWithoutDependencies;
            internal Action<IView[]> connectArray;
            internal Action<IView> disconnect;
            internal Action<IView[]> disconnectArray;
        }

        internal void ApplyConnector(Connector connector) {
            _connector = connector;
            connectState = ConnectState.Connected;
        }

        protected void InitSingle(ref bool token, Action init) {
            if (token) {
                return;
            }

            token = true;
            init();
        }
        
        public T ConnectView<T>(T view) where T : class, IView {
            TryApplyConnector(view);
            _connector.connect(view);
            return view;
        }
        
        public T ConnectView<T>(T view, UnloadPool pool) where T : class, IView {
            TryApplyConnector(view);
            _connector.connect(view);
            pool.Add(new UnloadAction(() => DisconnectView(view)));
            return view;
        }
        
        public void ConnectView(params IView[] views) {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                TryApplyConnector(views[viewId]);
            }
            
            _connector.connectArray(views);
        }
        
        public void ConnectView(params View[] views) {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                views[viewId].ApplyConnector(_connector);
            }
            
            _connector.connectArray(views);
        }
        
        public void ConnectView<T>(T[] views, params IDependency[] dependencies) where T : class, IView, IResolving {
            DependencyContainer container = new DependencyContainer(dependencies);
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                T view = views[viewId];
                TryApplyConnector(view);
                _connector.connectWithoutDependencies(view, () => ResolveUtility.Resolve(view, this, container));
            }
        }
        
        public void ConnectView(UnloadPool pool, params IView[] views) {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                TryApplyConnector(views[viewId]);
            }
            
            _connector.connectArray(views);
            
            foreach (IView view in views) {
                pool.Add(new UnloadAction(() => DisconnectView(view)));   
            }
        }
        
        public T ReconnectView<T>(T view, IDependency dependency) where T : View, IResolving {
            if (view.connectState == ConnectState.Connected) {
                DisconnectView(view);
            }
            
            return ConnectView(view, dependency);
        }
        
        public T ReconnectView<T>(T view, params IDependency[] dependencies) where T : View, IResolving {
            if (view.connectState == ConnectState.Connected) {
                DisconnectView(view);
            }
            
            return ConnectView(view, dependencies);
        }
        
        public T ConnectView<T>(T view, IDependency dependency) where T : class, IView, IResolving {
            TryApplyConnector(view);
            _connector.connectWithoutDependencies(view, () => ResolveUtility.Resolve(view, this, new DependencyContainer(dependency)));
            return view;
        }
        
        public T ConnectView<T>(T view, UnloadPool pool, IDependency dependency) where T : class, IView, IResolving {
            TryApplyConnector(view);
            _connector.connectWithoutDependencies(view, () => ResolveUtility.Resolve(view, this, new DependencyContainer(dependency)));
            pool.Add(new UnloadAction(() => DisconnectView(view)));
            return view;
        }
        
        public T ConnectView<T>(T view, params IDependency[] dependencies) where T : class, IView, IResolving {
            TryApplyConnector(view);
            _connector.connectWithoutDependencies(view, () => ResolveUtility.Resolve(view, this, new DependencyContainer(dependencies)));
            return view;
        }
        
        public T ConnectView<T>(T view, DependencyContainer container) where T : class, IView, IResolving {
            TryApplyConnector(view);
            _connector.connectWithoutDependencies(view, () => ResolveUtility.Resolve(view, this, container));
            return view;
        }
        
        public T ConnectView<T>(T view, UnloadPool pool, params IDependency[] dependencies) where T : class, IView, IResolving {
            TryApplyConnector(view);
            _connector.connectWithoutDependencies(view, () => ResolveUtility.Resolve(view, this, new DependencyContainer(dependencies)));
            pool.Add(new UnloadAction(() => DisconnectView(view)));
            return view;
        }
        
        protected T Bind<T>([NotNull] Binder<T> binder) where T : class, IDependency, new() => binder.GetDependency() as T;

        protected T Bind<T>([NotNull] Binder<T> binder, [NotNull] IDependency dependency) where T : class, IDependency, new() {
            ResolveUtility.Resolve(binder, this, new DependencyContainer(dependency));
            return binder.GetDependency() as T;
        }
        
        protected T Bind<T>([NotNull] Binder<T> binder, [NotNull] params IDependency[] dependencies) where T : class, IDependency, new() {
            ResolveUtility.Resolve(binder, this, new DependencyContainer(dependencies));
            return binder.GetDependency() as T;
        }

        public void DisconnectView<T>(T view) where T : class, IView {
            TryApplyDisconnectState(view);
            _connector.disconnect(view);
        }

        public void DisconnectView(params IView[] views) {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                TryApplyDisconnectState(views[viewId]);
            }
            
            _connector.disconnectArray(views);
        }

        private void TryApplyDisconnectState<T>(T view) where T : class, IView {
            if (view is View root) {
                root.connectState = ConnectState.Disconnected;
            }
        }
        
        private bool TryApplyConnector<T>(T controller) where T : class, IView {
            if (controller is not View root) {
                return false;
            }

            root.ApplyConnector(_connector);
            return true;
        }
    }
}