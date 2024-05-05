using System;
using JetBrains.Annotations;
using TinyMVC.Boot;
using TinyMVC.Dependencies;
using UnityEngine;

namespace TinyMVC.Views {
    public abstract class View : MonoBehaviour, IView {
        public ConnectState connectState { get; private set; }

        internal int sceneId;

        public enum ConnectState : byte {
            Disconnected,
            Connected
        }
        
        protected void InitSingle(ref bool token, Action init) {
            if (token) {
                return;
            }

            token = true;
            init();
        }

        public void ConnectView([NotNull] params IView[] views) {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                TryApply(views[viewId], ConnectState.Connected);
                SceneContext.GetContext(sceneId).Connect(views[viewId], sceneId, ProjectContext.data.Resolve);
            }
        }

        public T ConnectView<T>([NotNull] T view) where T : class, IView {
            TryApply(view, ConnectState.Connected);
            SceneContext.GetContext(sceneId).Connect(view, sceneId, ProjectContext.data.Resolve);
            return view;
        }

        public void ConnectView([NotNull] params View[] views) {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                TryApply(views[viewId], ConnectState.Connected);
                SceneContext.GetContext(sceneId).Connect(views[viewId], sceneId, ProjectContext.data.Resolve);
            }
        }
        
        public void ConnectView<T>(T[] views, params IDependency[] dependencies) where T : Component, IView, IResolving {
            DependencyContainer container = new DependencyContainer(dependencies);
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                TryApply(views[viewId], ConnectState.Connected);
                SceneContext.GetContext(sceneId).Connect(views[viewId], sceneId, resolving => ProjectContext.data.Resolve(container, resolving));
            }
        }
        
        public void ConnectView(View[] views, params IDependency[] dependencies) {
            DependencyContainer container = new DependencyContainer(dependencies);
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                TryApply(views[viewId], ConnectState.Connected);
                SceneContext.GetContext(sceneId).Connect(views[viewId], sceneId, resolving => ProjectContext.data.Resolve(container, resolving));
            }
        }
        
        public T ConnectView<T>([NotNull] T view, [NotNull] IDependency dependency) where T : Component, IView, IResolving {
            return ConnectView(view, new DependencyContainer(dependency));
        }
        
        public T ConnectView<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : Component, IView, IResolving {
            return ConnectView(view, new DependencyContainer(dependencies));
        }
        
        public T ConnectView<T>([NotNull] T view, [NotNull] DependencyContainer container) where T : Component, IView, IResolving {
            TryApply(view, ConnectState.Connected);
            SceneContext.GetContext(sceneId).Connect(view, sceneId, resolving => ProjectContext.data.Resolve(container, resolving));
            return view;
        }

        public T DisconnectView<T>(T view) where T : Component, IView {
            SceneContext.GetContext(sceneId).Disconnect(view, sceneId);
            TryApply(view, ConnectState.Disconnected);
            return view;
        }

        public void DisconnectView([NotNull] params IView[] views) {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                SceneContext.GetContext(sceneId).Disconnect(views[viewId], sceneId);
                TryApply(views[viewId], ConnectState.Disconnected);
            }
        }
        
        public T ReconnectView<T>([NotNull] T view, [NotNull] IDependency dependency) where T : View, IResolving {
            if (view.connectState == ConnectState.Connected) {
                DisconnectView(view);
            }
            
            return ConnectView(view, dependency);
        }
        
        public T ReconnectView<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : View, IResolving {
            if (view.connectState == ConnectState.Connected) {
                DisconnectView(view);
            }
            
            return ConnectView(view, dependencies);
        }
        
        private void TryApply<T>(T view, ConnectState state) where T : IView {
            if (view is View link) {
                link.sceneId = sceneId;
                link.connectState = state;
            }
        }
    }
}