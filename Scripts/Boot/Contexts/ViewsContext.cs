using System;
using System.Collections.Generic;
using TinyMVC.Dependencies;
using TinyMVC.Extensions;
using TinyMVC.Loop;
using TinyMVC.Views;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

using UnityObject = UnityEngine.Object;

namespace TinyMVC.Boot.Contexts {
    /// <summary> Contains references to scene objects </summary>
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideLabel]
#endif
    [Serializable]
    public abstract class ViewsContext {
        private List<IView> _mainViews;
        private List<IView> _subViews;
        
        internal void Create() {
            _mainViews = new List<IView>();
            _subViews = new List<IView>();
            Create(_mainViews);
        }

        internal void Init(Action<IView> connectView, Action<IView> disconnectView) {
            for (int viewId = 0; viewId < _mainViews.Count; viewId++) {
                if (_mainViews[viewId] is View view) {
                    view.ConnectToContext(connectView, disconnectView);
                }
            }
            
            _mainViews.TryInit();
        }

        internal void BeginPlay() => _mainViews.TryBeginPlay();
        
        internal void CheckAndAdd<T>(List<T> list) {
            list.Capacity += list.Count;
            
            for (int viewId = 0; viewId < _mainViews.Count; viewId++) {
                if (_mainViews[viewId] is T view) {
                    list.Add(view);
                }
            }
        }

        internal void ApplyDontDestroyOnLoad() {
            for (int viewId = 0; viewId < _mainViews.Count; viewId++) {
                if (_mainViews[viewId] is IDontDestroyOnLoad && _mainViews[viewId] is MonoBehaviour monoView) {
                    UnityObject.DontDestroyOnLoad(monoView);
                }
            }
        }
        
        internal void InitSubView(IView subView, Action<IResolving> resolve, Action<ILoop> addLoop) {
            if (subView is IInit init) {
                init.Init();
            }

            if (subView is IResolving resolving) {
                resolve(resolving);
            }

            if (subView is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }

            if (subView is ILoop loop) {
                addLoop(loop);
            }

            _subViews.Add(subView);
        }

        internal void DeInitSubView(IView subView, Action<ILoop> removeLoop) {
            if (subView is ILoop loop) {
                removeLoop(loop);
            }

            if (subView is IUnload unload) {
                unload.Unload();
            }

            _subViews.Remove(subView);
        }

        internal void Unload() {
            _subViews.TryUnload();
            _mainViews.TryUnload();
        }

        /// <summary> Instantiate new objects to scene before initialization process </summary>
        public abstract void Instantiate();
        
        /// <summary> Create and connect views to initialization </summary>
        /// <param name="views"> Any view type </param>
        protected abstract void Create(List<IView> views);

    #if UNITY_EDITOR && ODIN_INSPECTOR

        [Button("Generate", DirtyOnClick = true), HideInPlayMode, ShowIn(PrefabKind.InstanceInScene)]
        public virtual void Generate_Editor() {
            View[] views = UnityObject.FindObjectsOfType<View>(includeInactive: true);

            for (int viewId = 0; viewId < views.Length; viewId++) {
                views[viewId].Generate_Editor();
                UnityEditor.EditorUtility.SetDirty(views[viewId].gameObject);
            }
        }

    #endif
    }
}