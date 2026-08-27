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
        
        /// <summary> Creates an empty views context. </summary>
        public ViewsContextCore() => _views = new List<IView>();
        
        void IViewsContext.CreateViews() => _views.Clear();
        
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
        
        /// <summary> Runtime disconnection: Unload. </summary>
        internal void Disconnect(IView view) {
            if (view is IUnload unload) {
                unload.Unload();
            }
            
            _views.Remove(view);
        }
    }
}