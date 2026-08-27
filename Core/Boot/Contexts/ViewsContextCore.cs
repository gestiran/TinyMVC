// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Dependencies;
using TinyMVC.Dependencies.Extensions;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;
using TinyMVC.Views;
using TinyReactive;
using TinyReactive.Fields;

namespace TinyMVC.Boot.Contexts {
    /// <summary>
    /// Platform-independent views composition for <see cref="IView"/>.<br/>
    /// Runtime analog of the Unity <c>ViewsContext</c>. Designed for WPF/Desktop applications.<br/>
    /// All views are runtime-connected through <see cref="TinyMVC.Views.ViewExtension"/> or <c>Context.ConnectView</c>.
    /// </summary>
    internal sealed class ViewsContextCore : IViewsContext {
        /// <summary> Views connected at runtime through <see cref="TinyMVC.Views.ViewExtension"/> or <c>Context.ConnectView</c>. </summary>
        private readonly List<IView> _views;
        
        /// <summary>
        /// Pool of connected views: root view to its children.<br/>
        /// Analog of the Unity <c>WindowContext.connections</c> from the <c>WindowsService</c>. Stored like <see cref="ProjectComponents"/>.
        /// </summary>
        private readonly Dictionary<IView, List<IView>> _connections;
        
        private const int _CAPACITY = 64;
        
        /// <summary> Creates an empty views context. </summary>
        public ViewsContextCore() {
            _views = new List<IView>();
            _connections = new Dictionary<IView, List<IView>>(_CAPACITY);
        }
        
        void IViewsContext.CreateViews() {
            _views.Clear();
            _connections.Clear();
        }
        
        async Task IViewsContext.InitAsync() {
            for (int viewId = 0; viewId < _views.Count; viewId++) {
                _views[viewId].connectState = ConnectState.Connected;
            }
            
            await _views.TryInitAsync();
        }
        
        void IViewsContext.GetDependencies(List<IDependency> dependencies) {
            for (int viewId = 0; viewId < _views.Count; viewId++) {
                if (_views[viewId] is IDependency dependency) {
                    dependencies.Add(dependency);
                }
            }
        }
        
        void IViewsContext.TryApplyResolving() => _views.TryApplyResolving();
        
        async Task IViewsContext.BeginPlay() => await _views.TryBeginPlayAsync();
        
        void IViewsContext.Unload() {
            _views.TryUnload();
            _views.Clear();
            _connections.Clear();
        }
        
        void IViewsContext.Add(IView view) => _views.Add(view);
        
        /// <summary> Runtime connection: Init → ApplyResolving → BeginPlay. </summary>
        internal void Connect(IView view) {
            if (view is IInit init) {
                init.Init();
            }
            
            if (view is IApplyResolving applyResolving) {
                applyResolving.ApplyResolving();
            }
            
            if (view is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }
            
            _views.Add(view);
        }
        
        /// <summary> Runtime disconnection: Unload → recursive disconnection of all child views. </summary>
        internal void Disconnect(IView view) {
            if (view is IUnload unload) {
                unload.Unload();
            }
            
            DisconnectAll(view);
            _views.Remove(view);
        }
        
        /// <summary> Returns the children pool of the given root view. Creates one if it doesn't exist. </summary>
        internal List<IView> GetOrCreateConnections(IView root) {
            if (_connections.TryGetValue(root, out List<IView> connections) == false) {
                connections = new List<IView>();
                _connections.Add(root, connections);
            }
            
            return connections;
        }
        
        /// <summary> Tries to get the children pool of the given root view. </summary>
        internal bool TryGetConnections(IView root, out List<IView> connections) => _connections.TryGetValue(root, out connections);
        
        /// <summary> Removes a single child view from the root connections pool. </summary>
        internal void RemoveConnection(IView root, IView view) {
            if (_connections.TryGetValue(root, out List<IView> connections)) {
                connections.Remove(view);
                
                if (connections.Count == 0) {
                    _connections.Remove(root);
                }
            }
        }
        
        /// <summary> Recursively disconnects all child views connected to the given root. </summary>
        internal void DisconnectAll(IView root) {
            if (_connections.TryGetValue(root, out List<IView> connections) == false) {
                return;
            }
            
            for (int connectionId = connections.Count - 1; connectionId >= 0; connectionId--) {
                IView view = connections[connectionId];
                
                view.root = null;
                view.connectState = ConnectState.Disconnected;
                connections.RemoveAt(connectionId);
                Disconnect(view);
            }
            
            _connections.Remove(root);
        }
    }
}