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
        
        internal List<View> mainViews;
        internal List<View> subViews;
        
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
            for (int assetId = 0; assetId < mainViews.Count; assetId++) {
                if (mainViews[assetId] is IDependency dependency) {
                    dependencies.Add(dependency);
                }
            }
        }
        
        internal void CreateViews() {
            mainViews = new List<View>();
            subViews = new List<View>();
            
            Create();
            
            mainViews.AddRange(_assets);
            mainViews.AddRange(_generated);
        }
        
        internal async Task InitAsync() {
            for (int viewId = 0; viewId < mainViews.Count; viewId++) {
                mainViews[viewId].connectState = View.ConnectState.Connected;
            }
            
            await mainViews.TryInitAsync();
        }
        
        internal async Task BeginPlay() => await mainViews.TryBeginPlayAsync();
        
        internal void CheckAndAdd<T>(List<T> list) {
            for (int viewId = 0; viewId < mainViews.Count; viewId++) {
                if (mainViews[viewId] is T view) {
                    list.Add(view);
                }
            }
        }
        
        internal void ApplyDontDestroyOnLoad() {
            for (int viewId = 0; viewId < mainViews.Count; viewId++) {
                if (mainViews[viewId] is IDontDestroyOnLoad && mainViews[viewId] is MonoBehaviour monoView) {
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
            
            subViews.Add(view);
        }
        
        internal void Disconnect(View view, int sceneId) {
            if (view is ILoop loop) {
                ProjectContext.DisconnectLoop(sceneId, loop);
            }
            
            if (view is IUnload unload) {
                unload.Unload();
            }
            
            subViews.Remove(view);
        }
        
        internal void Unload() {
            subViews.TryUnload();
            mainViews.TryUnload();
        }
        
        protected void Add<T>(T view) where T : View => mainViews.Add(view);
        
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