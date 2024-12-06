using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityObject = UnityEngine.Object;

namespace TinyMVC.Boot.Contexts {
    [Serializable, InlineProperty, HideLabel]
    public abstract class ViewsContext {
        [InfoBox("Instantiated automatically after scene loaded.")]
        [SerializeField, ListDrawerSettings(HideAddButton = true, NumberOfItemsPerPage = 5), AssetsOnly, Searchable, HideInPlayMode, Required]
        private View[] _assets;
        
        [InfoBox("Auto found in scene by Generate button.")]
        [SerializeField, LabelText("Generated Assets"), ShowIn(PrefabKind.InstanceInScene), RequiredIn(PrefabKind.InstanceInScene), ReadOnly]
        private View[] _generated;
        
        private List<View> _mainViews;
        private List<View> _subViews;
        
        internal void Instantiate() {
            for (int assetId = 0; assetId < _assets.Length; assetId++) {
            #if UNITY_EDITOR
                if (_assets[assetId] == null) {
                    Debug.LogError("Context contain null element!");
                    continue;
                }
            #endif
                
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
            _mainViews = new List<View>();
            _subViews = new List<View>();
            
            Create();
            
            _mainViews.AddRange(_assets);
            _mainViews.AddRange(_generated);
        }
        
        internal async Task InitAsync() {
            for (int viewId = 0; viewId < _mainViews.Count; viewId++) {
                _mainViews[viewId].connectState = View.ConnectState.Connected;
            }
            
            await _mainViews.TryInitAsync();
        }
        
        internal async Task BeginPlay() => await _mainViews.TryBeginPlayAsync();
        
        internal void CheckAndAdd<T>(List<T> list) {
            list.Capacity += _mainViews.Count;
            
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
        
        internal void Connect(View view, int sceneId, Action<IResolving> resolve) {
            if (view is IInit init) {
                init.Init();
            }
            
            if (view is IResolving resolving) {
                resolve(resolving);
                
                if (view is IApplyResolving apply) {
                    apply.ApplyResolving();
                }
            }
            
            if (view is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }
            
            if (view is ILoop loop) {
                ProjectContext.ConnectLoop(sceneId, loop);
            }
            
            _subViews.Add(view);
        }
        
        internal void Disconnect(View view, int sceneId) {
            if (view is ILoop loop) {
                ProjectContext.DisconnectLoop(sceneId, loop);
            }
            
            if (view is IUnload unload) {
                unload.Unload();
            }
            
            _subViews.Remove(view);
        }
        
        internal void Unload() {
            _subViews.TryUnload();
            _mainViews.TryUnload();
        }
        
        protected void Add<T>(T view) where T : View => _mainViews.Add(view);
        
        protected abstract void Create();
        
    #if UNITY_EDITOR
        
        protected bool TryGetGenerated_Editor<T>(out T view) where T : View, IGeneratedContext {
            for (int i = 0; i < _generated.Length; i++) {
                if (_generated[i] is T result) {
                    view = result;
                    return true;
                }
            }
            
            view = null;
            return false;
        }
        
        public virtual void Reset() {
            View[] views = UnityObject.FindObjectsOfType<View>(true);
            List<View> generated = new List<View>();
            
            for (int viewId = 0; viewId < views.Length; viewId++) {
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