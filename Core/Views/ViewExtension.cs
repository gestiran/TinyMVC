// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyMVC.Boot;
using TinyMVC.Boot.Contexts;
using TinyMVC.Dependencies;

namespace TinyMVC.Views {
    /// <summary>
    /// Platform-independent connection API for <see cref="IView"/>.<br/>
    /// Runtime analog of the Unity <c>View</c> class. Designed for WPF/Desktop applications.<br/>
    /// The pool of connected views lives inside <see cref="ViewsContextCore"/> (analog of the <c>WindowsService</c> connections).
    /// </summary>
    public static class ViewExtension {
        [Obsolete("Can't connect nothing!", true)]
        public static void Connect(this IView _) { }
        
        [Obsolete("Can't connect nothing!", true)]
        public static void Connect(this IView _, string contextKey) { }
        
        /// <summary> Connects the view to the current context: Init → ApplyResolving → BeginPlay. </summary>
        public static T Connect<T>(this IView root, T view) where T : IView {
            return Connect(root, view, ProjectContext.scene.key);
        }
        
        /// <summary> Connects the view to the target context: Init → ApplyResolving → BeginPlay. </summary>
        public static T Connect<T>(this IView root, T view, string contextKey) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore views)) {
                view.root = root;
                view.connectState = ConnectState.Connected;
                views.GetOrCreateConnections(root).Add(view);
                views.Connect(view);
            }
            
            return view;
        }
        
        /// <summary> Connects the view with a single <paramref name="dependency"/>. </summary>
        public static T Connect<T>(this IView root, T view, IDependency dependency) where T : IView {
            return Connect(root, view, ProjectContext.scene.key, new DependencyContainer(dependency));
        }
        
        /// <summary> Connects the view with the resolved <paramref name="dependencies"/>. </summary>
        public static T Connect<T>(this IView root, T view, params IDependency[] dependencies) where T : IView {
            return Connect(root, view, ProjectContext.scene.key, new DependencyContainer(dependencies));
        }
        
        /// <summary> Connects the view with a resolved <paramref name="container"/>. </summary>
        public static T Connect<T>(this IView root, T view, DependencyContainer container) where T : IView {
            return Connect(root, view, ProjectContext.scene.key, container);
        }
        
        /// <summary> Connects the view to the target context with the resolved <paramref name="dependencies"/>. </summary>
        public static T Connect<T>(this IView root, T view, string contextKey, params IDependency[] dependencies) where T : IView {
            return Connect(root, view, contextKey, new DependencyContainer(dependencies));
        }
        
        /// <summary> Connects the view to the target context with a resolved <paramref name="container"/>. </summary>
        public static T Connect<T>(this IView root, T view, string contextKey, DependencyContainer container) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore views)) {
                ProjectContext.data.tempContainer = container;
                view.root = root;
                view.connectState = ConnectState.Connected;
                views.GetOrCreateConnections(root).Add(view);
                views.Connect(view);
            }
            
            return view;
        }
        
        /// <summary> Connects the views to the current context. </summary>
        public static void Connect<T>(this IView root, params T[] views) where T : IView {
            Connect(root, ProjectContext.scene.key, views);
        }
        
        /// <summary> Connects the views to the target context. </summary>
        public static void Connect<T>(this IView root, string contextKey, params T[] views) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore target) == false) {
                return;
            }
            
            List<IView> connections = target.GetOrCreateConnections(root);
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                views[viewId].root = root;
                views[viewId].connectState = ConnectState.Connected;
                connections.Add(views[viewId]);
                target.Connect(views[viewId]);
            }
        }
        
        /// <summary> Connects the views with the resolved <paramref name="dependencies"/>. </summary>
        public static void Connect<T>(this IView root, T[] views, params IDependency[] dependencies) where T : IView {
            Connect(root, views, ProjectContext.scene.key, dependencies);
        }
        
        /// <summary> Connects the views to the target context with the resolved <paramref name="dependencies"/>. </summary>
        public static void Connect<T>(this IView root, T[] views, string contextKey, params IDependency[] dependencies) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore target) == false) {
                return;
            }
            
            ProjectContext.data.tempContainer = new DependencyContainer(dependencies);
            
            List<IView> connections = target.GetOrCreateConnections(root);
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                views[viewId].root = root;
                views[viewId].connectState = ConnectState.Connected;
                connections.Add(views[viewId]);
                target.Connect(views[viewId]);
            }
        }
        
        /// <summary> Disconnects the view from its <see cref="IView.root"/>. All connected children are disconnected recursively. </summary>
        public static void Disconnect(this IView view) {
            if (view.connectState == ConnectState.Disconnected) {
                return;
            }
            
            if (view.root == null) {
                return;
            }
            
            Disconnect(view.root, view);
        }
        
        /// <summary> Disconnects the view from the target context. All connected children are disconnected recursively. </summary>
        public static void Disconnect(this IView view, string contextKey) {
            if (view.connectState == ConnectState.Disconnected) {
                return;
            }
            
            if (view.root == null) {
                return;
            }
            
            Disconnect(view.root, view, contextKey);
        }
        
        /// <summary> Disconnects the view from the current context: Unload → recursive disconnection of all connected children. </summary>
        public static T Disconnect<T>(this IView root, T view) where T : IView {
            return Disconnect(root, view, ProjectContext.scene.key);
        }
        
        /// <summary> Disconnects the view from the target context: Unload → recursive disconnection of all connected children. </summary>
        public static T Disconnect<T>(this IView root, T view, string contextKey) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore views)) {
                view.root = null;
                view.connectState = ConnectState.Disconnected;
                views.RemoveConnection(root, view);
                views.Disconnect(view);
            }
            
            return view;
        }
        
        /// <summary> Disconnects the views from the current context. </summary>
        public static void Disconnect<T>(this IView root, params T[] views) where T : IView {
            Disconnect(root, ProjectContext.scene.key, views);
        }
        
        /// <summary> Disconnects the views from the target context. </summary>
        public static void Disconnect<T>(this IView root, string contextKey, params T[] views) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore target) == false) {
                return;
            }
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                views[viewId].root = null;
                views[viewId].connectState = ConnectState.Disconnected;
                target.RemoveConnection(root, views[viewId]);
                target.Disconnect(views[viewId]);
            }
        }
        
        /// <summary> Reconnects the view with a single <paramref name="dependency"/>. </summary>
        public static T Reconnect<T>(this IView root, T view, IDependency dependency) where T : IView {
            if (view.connectState == ConnectState.Connected) {
                root.Disconnect(view);
            }
            
            return root.Connect(view, dependency);
        }
        
        /// <summary> Reconnects the view with the resolved <paramref name="dependencies"/>. </summary>
        public static T Reconnect<T>(this IView root, T view, params IDependency[] dependencies) where T : IView {
            if (view.connectState == ConnectState.Connected) {
                root.Disconnect(view);
            }
            
            return root.Connect(view, dependencies);
        }
        
        /// <summary> Calls <see cref="IUpdateConnection.UpdateConnection"/> on all connected views of the current context. </summary>
        public static void UpdateConnections(this IView root) => UpdateConnections(root, ProjectContext.scene.key);
        
        /// <summary> Calls <see cref="IUpdateConnection.UpdateConnection"/> on all connected views of the target context. </summary>
        public static void UpdateConnections(this IView root, string contextKey) {
            if (TryGetViewsContext(contextKey, out ViewsContextCore views) == false || views.TryGetConnections(root, out List<IView> connections) == false) {
                return;
            }
            
            for (int connectionId = 0; connectionId < connections.Count; connectionId++) {
                if (connections[connectionId] is IUpdateConnection update) {
                    update.UpdateConnection();
                }
            }
        }
        
        /// <summary> Calls <see cref="IUpdateConnection.UpdateConnection"/> on all connected views of the target type from the current context. </summary>
        public static void UpdateConnections<T>(this IView root) where T : IView => UpdateConnections<T>(root, ProjectContext.scene.key);
        
        /// <summary> Calls <see cref="IUpdateConnection.UpdateConnection"/> on all connected views of the target type from the target context. </summary>
        public static void UpdateConnections<T>(this IView root, string contextKey) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore views) == false || views.TryGetConnections(root, out List<IView> connections) == false) {
                return;
            }
            
            for (int connectionId = 0; connectionId < connections.Count; connectionId++) {
                IView view = connections[connectionId];
                
                if (view is not T) {
                    continue;
                }
                
                if (view is IUpdateConnection update) {
                    update.UpdateConnection();
                }
            }
        }
        
        /// <summary> Recursively disconnects all connected views from the current context. </summary>
        public static void DisconnectAll(this IView root) => DisconnectAll(root, ProjectContext.scene.key);
        
        /// <summary> Recursively disconnects all connected views from the target context. </summary>
        public static void DisconnectAll(this IView root, string contextKey) {
            if (TryGetViewsContext(contextKey, out ViewsContextCore views)) {
                views.DisconnectAll(root);
            }
        }
        
        /// <summary> Recursively disconnects all connected views of the target type from the current context. </summary>
        public static void DisconnectAll<T>(this IView root) where T : IView {
            DisconnectAll<T>(root, ProjectContext.scene.key);
        }
        
        /// <summary>
        /// Recursively disconnects all connected views of the target type from the target context.<br/>
        /// Children of the disconnected views are disconnected fully (like the Unity <c>WindowContext.Disconnect</c>).
        /// </summary>
        public static void DisconnectAll<T>(this IView root, string contextKey) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore views) == false) {
                return;
            }
            
            if (views.TryGetConnections(root, out List<IView> connections) == false) {
                return;
            }
            
            for (int connectionId = connections.Count - 1; connectionId >= 0; connectionId--) {
                IView view = connections[connectionId];
                
                if (view is not T) {
                    continue;
                }
                
                DisconnectAll<T>(view, contextKey);
                
                view.root = null;
                view.connectState = ConnectState.Disconnected;
                connections.RemoveAt(connectionId);
                views.Disconnect(view);
            }
        }
        
        private static bool TryGetViewsContext(string contextKey, out ViewsContextCore views) {
            if (ProjectContext.TryGetContext(contextKey, out IContext context) && context.views is ViewsContextCore target) {
                views = target;
                return true;
            }
            
            views = null;
            return false;
        }
    }
}