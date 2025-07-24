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
    public abstract class View : MonoBehaviour
#if ODIN_INSPECTOR && ODIN_VALIDATOR
                               , ISelfValidator
#endif
    {
        public ConnectState connectState { get; internal set; }
        
    #if ODIN_INSPECTOR
        [field: FoldoutGroup("Generated", 1000), ShowIn(PrefabKind.InstanceInScene | PrefabKind.InstanceInPrefab), ReadOnly]
    #endif
        [field: SerializeField]
        public View parent { get; private set; }
        
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
        
        [Obsolete("Can't disconnect nothing!", true)]
        public void Disconnect() {
            // Do nothing!
        }
        
        [Obsolete("Can't disconnect nothing!", true)]
        public void Disconnect(string contextKey) {
            // Do nothing!
        }
        
        public T Connect<T>([NotNull] T view) where T : View => Connect(view, ProjectContext.activeContext.key);
        
        public T Connect<T>([NotNull] T view, string contextKey) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                view.connectState = ConnectState.Connected;
                _connections.Add(view);
                context.Connect(view, ResolveUtility.Resolve);
            }
            
            return view;
        }
        
        public void Connect<T>([NotNull] params T[] views) where T : View {
            Connect(ProjectContext.activeContext.key, views);
        }
        
        public void Connect<T>([NotNull] T[] views, [NotNull] params IDependency[] dependencies) where T : View {
            Connect(views, ProjectContext.activeContext.key, dependencies);
        }
        
        public void Connect<T>([NotNull] T[] views, string contextKey, [NotNull] params IDependency[] dependencies) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context) == false) {
                return;
            }
            
            DependencyContainer container = new DependencyContainer(dependencies);
            ProjectContext.data.tempContainer = container;
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                views[viewId].connectState = ConnectState.Connected;
                _connections.Add(views[viewId]);
                context.Connect(views[viewId], resolving => ResolveUtility.Resolve(resolving, container));
            }
        }
        
        public void Connect<T>(string contextKey, [NotNull] params T[] views) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context) == false) {
                return;
            }
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                views[viewId].connectState = ConnectState.Connected;
                _connections.Add(views[viewId]);
                context.Connect(views[viewId], ResolveUtility.Resolve);
            }
        }
        
        public T Connect<T>([NotNull] T view, [NotNull] IDependency dependency) where T : View, IResolving {
            return Connect(view, ProjectContext.activeContext.key, new DependencyContainer(dependency));
        }
        
        public T Connect<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : View, IResolving {
            return Connect(view, ProjectContext.activeContext.key, new DependencyContainer(dependencies));
        }
        
        public T Connect<T>([NotNull] T view, [NotNull] DependencyContainer container) where T : View, IResolving {
            return Connect(view, ProjectContext.activeContext.key, container);
        }
        
        public T Connect<T>([NotNull] T view, string contextKey, [NotNull] DependencyContainer container) where T : View, IResolving {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                ProjectContext.data.tempContainer = container;
                view.connectState = ConnectState.Connected;
                _connections.Add(view);
                context.Connect(view, resolving => ResolveUtility.Resolve(resolving, container));
            }
            
            return view;
        }
        
        public T Insert<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : View {
            ProjectContext.data.tempContainer = new DependencyContainer(dependencies);
            return Insert(view, ProjectContext.activeContext.key);
        }
        
        public T Insert<T>([NotNull] T view) where T : View => Insert(view, ProjectContext.activeContext.key);
        
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
        
        public T Disconnect<T>(T view) where T : View => Disconnect(view, ProjectContext.activeContext.key);
        
        public T Disconnect<T>(T view, string contextKey) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                view.connectState = ConnectState.Disconnected;
                _connections.Remove(view);
                context.Disconnect(view);
            }
            
            return view;
        }
        
        public void Disconnect<T>([NotNull] params T[] views) where T : View => Disconnect(ProjectContext.activeContext.key, views);
        
        public void Disconnect<T>(string contextKey, [NotNull] params T[] views) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                for (int viewId = 0; viewId < views.Length; viewId++) {
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
            
            if (ProjectContext.TryGetContext(ProjectContext.activeContext.key, out SceneContext context)) {
                List<View> pool = new List<View>();
                
                for (int connectionId = 0; connectionId < _connections.Count; connectionId++) {
                    View view = _connections[connectionId];
                    
                    if (view is not T) {
                        continue;
                    }
                    
                    pool.Add(view);
                }
                
                for (int connectionId = 0; connectionId < pool.Count; connectionId++) {
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
            
            if (ProjectContext.TryGetContext(ProjectContext.activeContext.key, out SceneContext context)) {
                for (int connectionId = 0; connectionId < _connections.Count; connectionId++) {
                    View view = _connections[connectionId];
                    
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
                    
                    view.connectState = ConnectState.Disconnected;
                    context.Disconnect(view);
                }
                
                _connections.Clear();
            }
        }
        
        public T Reconnect<T>([NotNull] T view, [NotNull] IDependency dependency) where T : View, IResolving {
            if (view.connectState == ConnectState.Connected) {
                Disconnect(view);
            }
            
            return Connect(view, dependency);
        }
        
        public T Reconnect<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : View, IResolving {
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
        public virtual void Validate(SelfValidationResult result) {
        #if UNITY_EDITOR
            if (parent != null) {
                View actualParent = GetActualParent();
                
                try {
                    if (actualParent.GetInstanceID() != parent.GetInstanceID()) {
                        result.AddError("Invalid parent data!").WithFix("Find actual parent", FixParent);
                    }
                } catch (NullReferenceException) {
                    result.AddError("Invalid parent data!").WithFix("Find actual parent", FixParent);
                }
            }
        #endif
        }
    #endif
        
    #if UNITY_EDITOR
        
        public virtual void Reset() {
            parent = GetActualParent();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        private void FixParent() {
            parent = GetActualParent();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        private View GetActualParent() {
            int current = gameObject.GetInstanceID();
            View[] parents = GetComponentsInParent<View>(true);
            
            for (int i = 0; i < parents.Length; i++) {
                if (parents[i] == null) {
                    continue;
                }
                
                if (parents[i].gameObject.GetInstanceID() == current) {
                    continue;
                }
                
                return parents[i];
            }
            
            return null;
        }
        
    #endif
    }
}