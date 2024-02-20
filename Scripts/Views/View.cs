using System;
using JetBrains.Annotations;
using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using UnityEngine;

namespace TinyMVC.Views {
    public abstract class View : MonoBehaviour, IView {
        private Connector _connector;
        
        internal sealed class Connector {
            internal Action<IView> connect;
            internal Action<IView, Action> connectWithoutDependencies;
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
        
        protected T ConnectView<T>([NotNull] T view, UnloadPool pool) where T : class, IView {
            TryApplyConnector(view);
            _connector.connect(view);
            pool.Add(new UnloadAction(() => DisconnectView(view)));
            return view;
        }
        
        protected void ConnectView([NotNull] params IView[] views) {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                TryApplyConnector(views[viewId]);
            }
            
            _connector.connectArray(views);
        }
        
        protected void ConnectView(UnloadPool pool, [NotNull] params IView[] views) {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                TryApplyConnector(views[viewId]);
            }
            
            _connector.connectArray(views);
            
            foreach (IView view in views) {
                pool.Add(new UnloadAction(() => DisconnectView(view)));   
            }
        }
        
        protected T ConnectView<T>([NotNull] T view, IDependency dependency) where T : class, IView, IResolving {
            TryApplyConnector(view);
            _connector.connectWithoutDependencies(view, () => ResolveUtility.Resolve(view, new DependencyContainer(dependency)));
            return view;
        }
        
        protected T ConnectView<T>([NotNull] T view, UnloadPool pool, IDependency dependency) where T : class, IView, IResolving {
            TryApplyConnector(view);
            _connector.connectWithoutDependencies(view, () => ResolveUtility.Resolve(view, new DependencyContainer(dependency)));
            pool.Add(new UnloadAction(() => DisconnectView(view)));
            return view;
        }
        
        protected T ConnectView<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : class, IView, IResolving {
            TryApplyConnector(view);
            _connector.connectWithoutDependencies(view, () => ResolveUtility.Resolve(view, new DependencyContainer(dependencies)));
            return view;
        }
        
        protected T ConnectView<T>([NotNull] T view, UnloadPool pool, [NotNull] params IDependency[] dependencies) where T : class, IView, IResolving {
            TryApplyConnector(view);
            _connector.connectWithoutDependencies(view, () => ResolveUtility.Resolve(view, new DependencyContainer(dependencies)));
            pool.Add(new UnloadAction(() => DisconnectView(view)));
            return view;
        }

        protected T Bind<T>([NotNull] Binder<T> binder) where T : class, IDependency, new() => binder.GetDependency() as T;

        protected T Bind<T>([NotNull] Binder<T> binder, [NotNull] IDependency dependency) where T : class, IDependency, new() {
            ResolveUtility.Resolve(binder, new DependencyContainer(dependency));
            return binder.GetDependency() as T;
        }
        
        protected T Bind<T>([NotNull] Binder<T> binder, [NotNull] params IDependency[] dependencies) where T : class, IDependency, new() {
            ResolveUtility.Resolve(binder, new DependencyContainer(dependencies));
            return binder.GetDependency() as T;
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