using System;
using System.Collections.Generic;
using TinyMVC.Dependencies;
using TinyMVC.Extensions;
using TinyMVC.Loop;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
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
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [AssetsOnly, ListDrawerSettings(HideRemoveButton = true), AssetSelector(Paths = "Assets/Prefabs", ExpandAllMenuItems = false), Required]
    #endif
        [SerializeField]
        private View[] _assets;
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [SceneObjectsOnly, ShowIn(PrefabKind.InstanceInScene), ReadOnly, RequiredIn(PrefabKind.InstanceInScene)]
    #endif
        [SerializeField]
        private View[] _generated;
        
        private List<IView> _mainViews;
        private List<IView> _subViews;
        
        /// <summary> Instantiate new objects to scene before initialization process </summary>
        internal void Instantiate() {
            for (int assetId = 0; assetId < _assets.Length; assetId++) {
                _assets[assetId] = UnityObject.Instantiate(_assets[assetId]);
            }
        }

        internal void GetDependencies(List<IDependency> dependencies) {
            for (int assetId = 0; assetId < _assets.Length; assetId++) {
                if (_assets[assetId] is IDependency dependency) {
                    dependencies.Add(dependency);
                }
            }
            
            for (int generateId = 0; generateId < _generated.Length; generateId++) {
                if (_generated[generateId] is IDependency dependency) {
                    dependencies.Add(dependency);
                }
            }
        }
        
        internal void CreateViews() {
            _mainViews = new List<IView>();
            _subViews = new List<IView>();
            
            Create();
            
            _mainViews.AddRange(_assets);
            _mainViews.AddRange(_generated);
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

        protected void Add<T>(T view) where T : IView => _mainViews.Add(view);

        /// <summary> Create and connect views to initialization </summary>
        protected abstract void Create();

    #if UNITY_EDITOR && ODIN_INSPECTOR
        
        internal void Generate_Editor() {
            View[] views = UnityObject.FindObjectsOfType<View>(includeInactive: true);
            List<View> generated = new List<View>();
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
                if (views[viewId] is not IGenerated) {
                    continue;
                }

                if (views[viewId] is IGeneratedContext) {
                    generated.Add(views[viewId]); 
                }

                if (views[viewId] is IApplyGenerated target) {
                    target.Reset();
                    UnityEditor.EditorUtility.SetDirty(views[viewId].gameObject);
                } else if (views[viewId] is IApplyGeneratedContext targetContext) {
                    targetContext.Reset();
                    UnityEditor.EditorUtility.SetDirty(views[viewId].gameObject);
                }
            }
            
            _generated = generated.ToArray();
        }

    #endif
    }
}