// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TinyMVC.Boot;
using TinyMVC.Boot.Contexts;
using TinyMVC.Dependencies;
using TinyReactive;

namespace TinyMVC.Views {
    /// <summary>
    /// Platform-independent connection API for <see cref="IView"/>.<br/>
    /// Runtime analog of the Unity <c>View</c> class. Designed for WPF/Desktop applications.<br/>
    /// The pool of connected views lives inside <see cref="ViewsContextCore"/> (analog of the <c>WindowsService</c> connections).
    /// </summary>
    public static class ViewExtension {
        [Pure]
        public static IEnumerable<T> Connections<T>(this IView root) where T : IView {
            return root.Connections<T>(ProjectContext.scene.key);
        }
        
        [Pure]
        public static IEnumerable<T> Connections<T>(this IView root, string contextKey) where T : IView {
            foreach (IView view in root.Connections(contextKey)) {
                if (view is T target) {
                    yield return target;
                }
            }
        }
        
        [Pure]
        public static IEnumerable<IView> Connections(this IView root) {
            return root.Connections(ProjectContext.scene.key);
        }
        
        [Pure]
        public static IEnumerable<IView> Connections(this IView root, string contextKey) {
            if (TryGetViewsContext(contextKey, out ViewsContextCore context) && context.TryGetConnections(root, out List<IView> connections)) {
                for (int connectionId = connections.Count - 1; connectionId >= 0; connectionId--) {
                    yield return connections[connectionId];
                }
            }
        }
        
        /// <summary> Connects the view to the current context: Init → ApplyResolving → BeginPlay. </summary>
        public static T Connect<T>(this IView root, T view) where T : IView {
            return root.Connect(view, ProjectContext.scene.key);
        }
        
        /// <summary> Connects the view to the target context: Init → ApplyResolving → BeginPlay. </summary>
        public static T Connect<T>(this IView root, T view, string contextKey) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore context) && view.connectState == ConnectState.Disconnected) {
                view.root = root;
                view.connectState = ConnectState.Connected;
                context.GetOrCreateConnections(root).Add(view);
                context.Connect(view);
            }
            
            return view;
        }
        
        /// <summary> Connects the view to the current context. Unloading the <paramref name="unload"/> pool disconnects the view on demand. </summary>
        public static T Connect<T>(this IView root, T view, IUnloadLink unload) where T : IView {
            return root.Connect(view, unload, ProjectContext.scene.key);
        }
        
        /// <summary> Connects the view to the target context. Unloading the <paramref name="unload"/> pool disconnects the view on demand. </summary>
        public static T Connect<T>(this IView root, T view, IUnloadLink unload, string contextKey) where T : IView {
            root.Connect(view, contextKey);
            
            if (unload != null) {
                unload.Add(new UnloadAction(() => view.Disconnect(contextKey)));
            }
            
            return view;
        }
        
        /// <summary> Connects the view with a single <paramref name="dependency"/>. </summary>
        public static T Connect<T>(this IView root, T view, IDependency dependency) where T : IView {
            return root.Connect(view, ProjectContext.scene.key, new DependencyContainer(dependency));
        }
        
        /// <summary> Connects the view with the resolved <paramref name="dependencies"/>. </summary>
        public static T Connect<T>(this IView root, T view, params IDependency[] dependencies) where T : IView {
            return root.Connect(view, ProjectContext.scene.key, new DependencyContainer(dependencies));
        }
        
        /// <summary> Connects the view with a resolved <paramref name="container"/>. </summary>
        public static T Connect<T>(this IView root, T view, DependencyContainer container) where T : IView {
            return root.Connect(view, ProjectContext.scene.key, container);
        }
        
        /// <summary> Connects the view to the target context with the resolved <paramref name="dependencies"/>. </summary>
        public static T Connect<T>(this IView root, T view, string contextKey, params IDependency[] dependencies) where T : IView {
            return root.Connect(view, contextKey, new DependencyContainer(dependencies));
        }
        
        /// <summary> Connects the view to the target context with a resolved <paramref name="container"/>. </summary>
        public static T Connect<T>(this IView root, T view, string contextKey, DependencyContainer container) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore context) && view.connectState == ConnectState.Disconnected) {
                ProjectContext.data.tempContainer = container;
                view.root = root;
                view.connectState = ConnectState.Connected;
                context.GetOrCreateConnections(root).Add(view);
                context.Connect(view);
            }
            
            return view;
        }
        
        /// <summary> Connects the views to the current context. </summary>
        public static void Connect<T>(this IView root, params T[] views) where T : IView {
            root.Connect(ProjectContext.scene.key, views);
        }
        
        /// <summary> Connects the views to the target context. </summary>
        public static void Connect<T>(this IView root, string contextKey, params T[] views) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore context)) {
                List<IView> connections = context.GetOrCreateConnections(root);
                
                for (int viewId = 0; viewId < views.Length; viewId++) {
                    T view = views[viewId];
                    
                    if (view.connectState == ConnectState.Disconnected) {
                        view.root = root;
                        view.connectState = ConnectState.Connected;
                        connections.Add(view);
                        context.Connect(view);
                    }
                }
            }
        }
        
        /// <summary> Connects the views with the resolved <paramref name="dependencies"/>. </summary>
        public static void Connect<T>(this IView root, T[] views, params IDependency[] dependencies) where T : IView {
            root.Connect(views, ProjectContext.scene.key, dependencies);
        }
        
        /// <summary> Connects the views to the target context with the resolved <paramref name="dependencies"/>. </summary>
        public static void Connect<T>(this IView root, T[] views, string contextKey, params IDependency[] dependencies) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore context)) {
                ProjectContext.data.tempContainer = new DependencyContainer(dependencies);
                
                List<IView> connections = context.GetOrCreateConnections(root);
                
                for (int viewId = 0; viewId < views.Length; viewId++) {
                    T view = views[viewId];
                    
                    if (view.connectState == ConnectState.Disconnected) {
                        view.root = root;
                        view.connectState = ConnectState.Connected;
                        connections.Add(view);
                        context.Connect(view);
                    }
                }
            }
        }
        
        /// <summary> Disconnects the view from its <see cref="IView.root"/>. All connected children are disconnected recursively. </summary>
        public static void Disconnect(this IView view) {
            if (view.connectState == ConnectState.Connected) {
                view.root.Disconnect(view);
            }
        }
        
        /// <summary> Disconnects the view from the target context. All connected children are disconnected recursively. </summary>
        public static void Disconnect(this IView view, string contextKey) {
            if (view.connectState == ConnectState.Connected) {
                view.root.Disconnect(view, contextKey);
            }
        }
        
        /// <summary> Disconnects the view from the current context: Unload → recursive disconnection of all connected children. </summary>
        public static T Disconnect<T>(this IView root, T view) where T : IView {
            return root.Disconnect(view, ProjectContext.scene.key);
        }
        
        /// <summary> Disconnects the view from the target context: Unload → recursive disconnection of all connected children. </summary>
        public static T Disconnect<T>(this IView root, T view, string contextKey) where T : IView {
            if (view.connectState == ConnectState.Connected && TryGetViewsContext(contextKey, out ViewsContextCore context)) {
                view.root = null;
                view.connectState = ConnectState.Disconnected;
                context.RemoveConnection(root, view);
                context.Disconnect(view);
            }
            
            return view;
        }
        
        /// <summary> Disconnects the views from the current context. </summary>
        public static void Disconnect<T>(this IView root, params T[] views) where T : IView {
            root.Disconnect(ProjectContext.scene.key, views);
        }
        
        /// <summary> Disconnects the views from the target context. </summary>
        public static void Disconnect<T>(this IView root, string contextKey, params T[] views) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore context) == false) {
                return;
            }
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                T view = views[viewId];
                
                if (view.connectState == ConnectState.Connected) {
                    view.root = null;
                    view.connectState = ConnectState.Disconnected;
                    context.RemoveConnection(root, view);
                    context.Disconnect(view);
                }
            }
        }
        
        public static bool DisconnectReference<T>(this IView root, T dependency, out IView connection) {
            return root.DisconnectReference(ProjectContext.scene.key, dependency, out connection);
        }
        
        public static bool DisconnectReference<T>(this IView root, string contextKey, T dependency, out IView connection) {
            if (TryGetViewsContext(contextKey, out ViewsContextCore context) && context.TryGetConnections(root, out List<IView> connections)) {
                for (int connectionId = connections.Count - 1; connectionId >= 0; connectionId--) {
                    connection = connections[connectionId];
                    
                    if (connection.connectState == ConnectState.Connected && connection is IEquatable<T> equatable && equatable.Equals(dependency)) {
                        connection.root = null;
                        connection.connectState = ConnectState.Disconnected;
                        context.RemoveConnection(root, connection);
                        context.Disconnect(connection);
                        return true;
                    }
                }
            }
            
            connection = null;
            return false;
        }
        
        public static void DisconnectReferences<T>(this IView root, T dependency) {
            root.DisconnectReferences(ProjectContext.scene.key, dependency);
        }
        
        public static void DisconnectReferences<T>(this IView root, string contextKey, T dependency) {
            if (TryGetViewsContext(contextKey, out ViewsContextCore context) && context.TryGetConnections(root, out List<IView> connections)) {
                for (int connectionId = connections.Count - 1; connectionId >= 0; connectionId--) {
                    IView view = connections[connectionId];
                    
                    if (view.connectState == ConnectState.Connected && view is IEquatable<T> equatable && equatable.Equals(dependency)) {
                        view.root = null;
                        view.connectState = ConnectState.Disconnected;
                        context.RemoveConnection(root, view);
                        context.Disconnect(view);
                    }
                }
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
            if (TryGetViewsContext(contextKey, out ViewsContextCore views) && views.TryGetConnections(root, out List<IView> connections)) {
                for (int connectionId = 0; connectionId < connections.Count; connectionId++) {
                    if (connections[connectionId] is IUpdateConnection update) {
                        update.UpdateConnection();
                    }
                }
            }
            
        }
        
        /// <summary> Calls <see cref="IUpdateConnection.UpdateConnection"/> on all connected views of the target type from the current context. </summary>
        public static void UpdateConnections<T>(this IView root) where T : IView => UpdateConnections<T>(root, ProjectContext.scene.key);
        
        /// <summary> Calls <see cref="IUpdateConnection.UpdateConnection"/> on all connected views of the target type from the target context. </summary>
        public static void UpdateConnections<T>(this IView root, string contextKey) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore views) && views.TryGetConnections(root, out List<IView> connections)) {
                for (int connectionId = 0; connectionId < connections.Count; connectionId++) {
                    IView view = connections[connectionId];
                    
                    if (view is T && view is IUpdateConnection update) {
                        update.UpdateConnection();
                    }
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
            root.DisconnectAll<T>(ProjectContext.scene.key);
        }
        
        /// <summary>
        /// Recursively disconnects all connected views of the target type from the target context.<br/>
        /// Children of the disconnected views are disconnected fully (like the Unity <c>WindowContext.Disconnect</c>).
        /// </summary>
        public static void DisconnectAll<T>(this IView root, string contextKey) where T : IView {
            if (TryGetViewsContext(contextKey, out ViewsContextCore views) && views.TryGetConnections(root, out List<IView> connections)) {
                for (int connectionId = connections.Count - 1; connectionId >= 0; connectionId--) {
                    IView view = connections[connectionId];
                    
                    if (view is T && view.connectState == ConnectState.Connected) {
                        view.DisconnectAll<T>(contextKey);
                        
                        view.root = null;
                        view.connectState = ConnectState.Disconnected;
                        connections.RemoveAt(connectionId);
                        views.Disconnect(view);
                    }
                }
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
        
        [Obsolete("Can't connect nothing!", true)]
        public static void Connect(this IView _) { }
        
        [Obsolete("Can't connect nothing!", true)]
        public static void Connect(this IView _, string contextKey) { }
    }
}