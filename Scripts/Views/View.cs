// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TinyMVC.Boot;
using TinyMVC.Dependencies;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Views {
#if ODIN_INSPECTOR && ODIN_VALIDATOR
    public abstract class View : MonoBehaviour, ISelfValidator
#else
    public abstract class View : MonoBehaviour
#endif
    {
        public View root { get; internal set; }
        public ConnectState connectState { get; internal set; }
        
        private readonly List<View> _connections = new List<View>();
        
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
        
        [Obsolete("Can't connect nothing!", true)]
        public void Connect() {
            // Do nothing!
        }
        
        [Obsolete("Can't connect nothing!", true)]
        public void Connect(string contextKey) {
            // Do nothing!
        }
        
        [Obsolete("Can't insert nothing!", true)]
        public void Insert() {
            // Do nothing!
        }
        
        [Obsolete("Can't insert nothing!", true)]
        public void Insert(string contextKey) {
            // Do nothing!
        }
        
        public T Connect<T>([NotNull] T view) where T : View => Connect(view, ProjectContext.scene.key);
        
        public T Connect<T>([NotNull] T view, string contextKey) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                view.root = this;
                view.connectState = ConnectState.Connected;
                _connections.Add(view);
                context.Connect(view);
            }
            
            return view;
        }
        
        public void Connect<T>([NotNull] params T[] views) where T : View {
            Connect(ProjectContext.scene.key, views);
        }
        
        public void Connect<T>([NotNull] T[] views, [NotNull] params IDependency[] dependencies) where T : View {
            Connect(views, ProjectContext.scene.key, dependencies);
        }
        
        public void Connect<T>([NotNull] T[] views, string contextKey, [NotNull] params IDependency[] dependencies) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context) == false) {
                return;
            }
            
            DependencyContainer container = new DependencyContainer(dependencies);
            ProjectContext.data.tempContainer = container;
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                views[viewId].root = this;
                views[viewId].connectState = ConnectState.Connected;
                _connections.Add(views[viewId]);
                context.Connect(views[viewId]);
            }
        }
        
        public void Connect<T>(string contextKey, [NotNull] params T[] views) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context) == false) {
                return;
            }
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                views[viewId].root = this;
                views[viewId].connectState = ConnectState.Connected;
                _connections.Add(views[viewId]);
                context.Connect(views[viewId]);
            }
        }
        
        public T Connect<T>([NotNull] T view, [NotNull] IDependency dependency) where T : View {
            return Connect(view, ProjectContext.scene.key, new DependencyContainer(dependency));
        }
        
        public T Connect<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : View {
            return Connect(view, ProjectContext.scene.key, new DependencyContainer(dependencies));
        }
        
        public T Connect<T>([NotNull] T view, [NotNull] DependencyContainer container) where T : View {
            return Connect(view, ProjectContext.scene.key, container);
        }
        
        public T Connect<T>([NotNull] T view, string contextKey, [NotNull] DependencyContainer container) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                ProjectContext.data.tempContainer = container;
                view.root = this;
                view.connectState = ConnectState.Connected;
                _connections.Add(view);
                context.Connect(view);
            }
            
            return view;
        }
        
        public T Insert<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : View {
            ProjectContext.data.tempContainer = new DependencyContainer(dependencies);
            return Insert(view, ProjectContext.scene.key);
        }
        
        public T Insert<T>([NotNull] T view) where T : View => Insert(view, ProjectContext.scene.key);
        
        public T Insert<T>([NotNull] T view, string contextKey, [NotNull] params IDependency[] dependencies) where T : View {
            ProjectContext.data.tempContainer = new DependencyContainer(dependencies);
            return Insert(view, contextKey);
        }
        
        public T Insert<T>([NotNull] T view, string contextKey) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                context.views.Insert(view);
            }
            
            return view;
        }
        
        public void Disconnect() {
            if (connectState == ConnectState.Disconnected) {
                return;
            }
            
            root.Disconnect(this);
        }
        
        public void Disconnect(string contextKey) {
            if (connectState == ConnectState.Disconnected) {
                return;
            }
            
            root.Disconnect(this, contextKey);
        }
        
        public T Disconnect<T>(T view) where T : View => Disconnect(view, ProjectContext.scene.key);
        
        public T Disconnect<T>(T view, string contextKey) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                view.root = null;
                view.connectState = ConnectState.Disconnected;
                _connections.Remove(view);
                context.Disconnect(view);
            }
            
            return view;
        }
        
        public void Disconnect<T>([NotNull] params T[] views) where T : View => Disconnect(ProjectContext.scene.key, views);
        
        public void Disconnect<T>(string contextKey, [NotNull] params T[] views) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                for (int viewId = 0; viewId < views.Length; viewId++) {
                    views[viewId].root = null;
                    views[viewId].connectState = ConnectState.Disconnected;
                    _connections.Remove(views[viewId]);
                    context.Disconnect(views[viewId]);
                }
            }
        }
        
        public void DisconnectAll<T>() where T : View {
            if (_connections.Count == 0) {
                return;
            }
            
            if (ProjectContext.TryGetContext(ProjectContext.scene.key, out SceneContext context)) {
                List<View> pool = new List<View>();
                
                for (int connectionId = 0; connectionId < _connections.Count; connectionId++) {
                    View view = _connections[connectionId];
                    
                    if (view is not T) {
                        continue;
                    }
                    
                    pool.Add(view);
                }
                
                for (int connectionId = 0; connectionId < pool.Count; connectionId++) {
                    pool[connectionId].root = null;
                    pool[connectionId].connectState = ConnectState.Disconnected;
                    _connections.Remove(pool[connectionId]);
                    context.Disconnect(pool[connectionId]);
                }
            }
        }
        
        public void DisconnectAll<T>(string contextKey) where T : View {
            if (_connections.Count == 0) {
                return;
            }
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                List<View> pool = new List<View>();
                
                for (int connectionId = 0; connectionId < _connections.Count; connectionId++) {
                    View view = _connections[connectionId];
                    
                    if (view is not T) {
                        continue;
                    }
                    
                    pool.Add(view);
                }
                
                for (int connectionId = 0; connectionId < pool.Count; connectionId++) {
                    pool[connectionId].root = null;
                    pool[connectionId].connectState = ConnectState.Disconnected;
                    _connections.Remove(pool[connectionId]);
                    context.Disconnect(pool[connectionId]);
                }
            }
        }
        
        public void DisconnectAll() {
            if (_connections.Count == 0) {
                return;
            }
            
            if (ProjectContext.TryGetContext(ProjectContext.scene.key, out SceneContext context)) {
                for (int connectionId = 0; connectionId < _connections.Count; connectionId++) {
                    View view = _connections[connectionId];
                    
                    view.root = null;
                    view.connectState = ConnectState.Disconnected;
                    context.Disconnect(view);
                }
                
                _connections.Clear();
            }
        }
        
        public void DisconnectAll(string contextKey) {
            if (_connections.Count == 0) {
                return;
            }
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                for (int connectionId = 0; connectionId < _connections.Count; connectionId++) {
                    View view = _connections[connectionId];
                    
                    view.root = null;
                    view.connectState = ConnectState.Disconnected;
                    context.Disconnect(view);
                }
                
                _connections.Clear();
            }
        }
        
        public T Reconnect<T>([NotNull] T view, [NotNull] IDependency dependency) where T : View {
            if (view.connectState == ConnectState.Connected) {
                Disconnect(view);
            }
            
            return Connect(view, dependency);
        }
        
        public T Reconnect<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : View {
            if (view.connectState == ConnectState.Connected) {
                Disconnect(view);
            }
            
            return Connect(view, dependencies);
        }
        
        public void UpdateConnections<T>() where T : View {
            for (int connectionId = 0; connectionId < _connections.Count; connectionId++) {
                View view = _connections[connectionId];
                
                if (view is not T) {
                    continue;
                }
                
                if (view is IUpdateConnection update) {
                    update.UpdateConnection();
                }
            }
        }
        
        public void UpdateConnections() {
            for (int connectionId = 0; connectionId < _connections.Count; connectionId++) {
                if (_connections[connectionId] is IUpdateConnection update) {
                    update.UpdateConnection();
                }
            }
        }
        
    #if ODIN_INSPECTOR && ODIN_VALIDATOR
        public virtual void Validate(SelfValidationResult result) { }
    #endif
        
    #if UNITY_EDITOR
        
        public virtual void Reset() => UnityEditor.EditorUtility.SetDirty(this);
        
    #endif
    }
}