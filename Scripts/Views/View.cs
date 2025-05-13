using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TinyMVC.Boot;
using TinyMVC.Dependencies;
using UnityEngine;

namespace TinyMVC.Views {
    public abstract class View : MonoBehaviour, ISelfValidator {
        public ConnectState connectState { get; internal set; }
        
        [field: SerializeField, FoldoutGroup("Generated", 1000), ShowIn(PrefabKind.InstanceInScene | PrefabKind.InstanceInPrefab), ReadOnly]
        public View parent { get; private set; }
        
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
        public static void Connect() {
            // Do nothing!
        }
        
        [Obsolete("Can't connect nothing!", true)]
        public static void Connect(int sceneId) {
            // Do nothing!
        }
        
        [Obsolete("Can't disconnect nothing!", true)]
        public static void Disconnect() {
            // Do nothing!
        }
        
        [Obsolete("Can't disconnect nothing!", true)]
        public static void Disconnect(int sceneId) {
            // Do nothing!
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Connect<T>([NotNull] T view) where T : View => Connect(view, ProjectContext.activeContext.key);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Connect<T>([NotNull] T view, string contextKey) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                view.connectState = ConnectState.Connected;
                context.Connect(view, ResolveUtility.Resolve);
            }
            
            return view;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T>([NotNull] params T[] views) where T : View {
            Connect(ProjectContext.activeContext.key, views);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T>([NotNull] T[] views, [NotNull] params IDependency[] dependencies) where T : View {
            Connect(views, ProjectContext.activeContext.key, dependencies);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T>([NotNull] T[] views, string contextKey, [NotNull] params IDependency[] dependencies) where T : View {
            DependencyContainer container = new DependencyContainer(dependencies);
            ProjectContext.data.tempContainer = container;
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                    context.Connect(views[viewId], resolving => ResolveUtility.Resolve(resolving, container));
                    views[viewId].connectState = ConnectState.Connected;
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T>(string contextKey, [NotNull] params T[] views) where T : View {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                    context.Connect(views[viewId], ResolveUtility.Resolve);
                    views[viewId].connectState = ConnectState.Connected;
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Connect<T>([NotNull] T view, [NotNull] IDependency dependency) where T : View, IResolving {
            return Connect(view, new DependencyContainer(dependency));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Connect<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : View, IResolving {
            return Connect(view, new DependencyContainer(dependencies));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Connect<T>([NotNull] T view, [NotNull] DependencyContainer container) where T : View, IResolving {
            return Connect(view, ProjectContext.activeContext.key, container);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Connect<T>([NotNull] T view, string contextKey, [NotNull] DependencyContainer container) where T : View, IResolving {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                ProjectContext.data.tempContainer = container;
                context.Connect(view, resolving => ResolveUtility.Resolve(resolving, container));
                view.connectState = ConnectState.Connected;
            }
            
            return view;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Disconnect<T>(T view) where T : View => Disconnect(view, ProjectContext.activeContext.key);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Disconnect<T>(T view, string contextKey) where T : View {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                context.Disconnect(view);
                view.connectState = ConnectState.Disconnected;
            }
            
            return view;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T>([NotNull] params T[] views) where T : View {
            Disconnect(ProjectContext.activeContext.key, views);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T>(string contextKey, [NotNull] params T[] views) where T : View {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                    context.Disconnect(views[viewId]);
                    views[viewId].connectState = ConnectState.Disconnected;
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Reconnect<T>([NotNull] T view, [NotNull] IDependency dependency) where T : View, IResolving {
            if (view.connectState == ConnectState.Connected) {
                Disconnect(view);
            }
            
            return Connect(view, dependency);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Reconnect<T>([NotNull] T view, [NotNull] params IDependency[] dependencies) where T : View, IResolving {
            if (view.connectState == ConnectState.Connected) {
                Disconnect(view);
            }
            
            return Connect(view, dependencies);
        }
        
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
            int current = GetComponent<View>().GetInstanceID();
            View[] parents = GetComponentsInParent<View>(true);
            
            for (int i = 0; i < parents.Length; i++) {
                if (parents[i] == null) {
                    continue;
                }
                
                if (parents[i].GetInstanceID() == current) {
                    continue;
                }
                
                return parents[i];
            }
            
            return null;
        }
        
    #endif
    }
}