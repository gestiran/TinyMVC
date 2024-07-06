using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Debugging;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;
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
        [ListDrawerSettings(HideAddButton = true, ShowFoldout = false), Searchable, Required]
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
            for (int assetId = 0; assetId < _mainViews.Count; assetId++) {
                if (_mainViews[assetId] is IDependency dependency) {
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
        
        internal async Task InitAsync(int sceneId) {
            for (int viewId = 0; viewId < _mainViews.Count; viewId++) {
                if (_mainViews[viewId] is View view) {
                    view.sceneId = sceneId;
                }
            }
            
            await _mainViews.TryInitAsync();
        }
        
        internal async Task BeginPlay() => await _mainViews.TryBeginPlayAsync();
        
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
        
        internal void Connect(IView view, int sceneId, Action<IResolving> resolve) {
            if (view is IInit init) {
                DebugUtility.CheckAndLogException(init.Init);
            }
            
            if (view is IResolving resolving) {
                resolve(resolving);
            }
            
            if (view is IBeginPlay beginPlay) {
                DebugUtility.CheckAndLogException(beginPlay.BeginPlay);
            }
            
            if (view is ILoop loop) {
                ProjectContext.current.ConnectLoop(sceneId, loop);
            }
            
            _subViews.Add(view);
        }
        
        internal void Disconnect(IView view, int sceneId) {
            if (view is ILoop loop) {
                ProjectContext.current.DisconnectLoop(sceneId, loop);
            }
            
            if (view is IUnload unload) {
                DebugUtility.CheckAndLogException(unload.Unload);
            }
            
            _subViews.Remove(view);
        }
        
        internal void Unload() {
            _subViews.TryUnload();
            _mainViews.TryUnload();
        }
        
        protected void Add<T>(T view) where T : IView => _mainViews.Add(view);
        
        /// <summary> Create and connect views to initialization </summary>
        protected abstract void Create();
        
        protected virtual void Generate() { }
        
        #if UNITY_EDITOR
        
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
            Generate();
        }
        
        #endif
    }
}