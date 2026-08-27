// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using TinyMVC.Dependencies;
using TinyMVC.Dependencies.Extensions;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using TinyReactive;
using TinyReactive.Fields;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot.Contexts {
#if ODIN_INSPECTOR
    [InlineProperty, HideLabel]
#endif
    [Serializable]
    public sealed class ViewsContext : IViewsContext, IInit {
    #if ODIN_INSPECTOR
        [InfoBox("Instantiated automatically after scene loaded.")]
        [ListDrawerSettings(HideAddButton = true, NumberOfItemsPerPage = 5), AssetsOnly, Searchable, HideInPlayMode, Required]
    #endif
        [SerializeField]
        private View[] _assets;
        
    #if ODIN_INSPECTOR
        [InfoBox("Auto found in scene by Generate button.")]
        [LabelText("Generated Assets"), ShowIn(PrefabKind.InstanceInScene), RequiredIn(PrefabKind.InstanceInScene), ReadOnly]
    #endif
        [SerializeField]
        private View[] _generated;
        
        private View[] _instances;
        internal List<View> mainViews;
        internal List<View> subViews;
        private bool _isUsedViewResolve;
        
        public void Init() {
            List<View> instances = new List<View>(_assets.Length);
            
            for (int assetId = 0; assetId < _assets.Length; assetId++) {
            #if UNITY_EDITOR
                if (_assets[assetId] == null) {
                    Debug.LogError("Context contain null element!");
                    continue;
                }
            #endif
                
                instances.Add(UnityObject.Instantiate(_assets[assetId]));
            }
            
            _instances = instances.ToArray();
            _isUsedViewResolve = false;
        }
        
        void IViewsContext.CreateViews() {
            mainViews = new List<View>();
            subViews = new List<View>();
            
            mainViews.AddRange(_instances);
            mainViews.AddRange(_generated);
        }
        
        async Task IViewsContext.InitAsync() {
            for (int viewId = 0; viewId < mainViews.Count; viewId++) {
                mainViews[viewId].connectState = ConnectState.Connected;
            }
            
            await mainViews.TryInitAsync();
        }
        
        void IViewsContext.GetDependencies(List<IDependency> dependencies) {
            for (int assetId = 0; assetId < mainViews.Count; assetId++) {
                if (mainViews[assetId] is IDependency dependency) {
                    dependencies.Add(dependency);
                }
            }
            
            _isUsedViewResolve = true;
        }
        
        void IViewsContext.TryApplyResolving() => mainViews.TryApplyResolving();
        
        async Task IViewsContext.BeginPlay() => await mainViews.TryBeginPlayAsync();
        
        void IViewsContext.Unload() {
            subViews.TryUnload();
            mainViews.TryUnload();
            
            subViews.Clear();
            mainViews.Clear();
        }
        
        void IViewsContext.Add(IView view) => mainViews.Add(view as View);
        
        internal void ApplyDontDestroyOnLoad() {
            for (int viewId = 0; viewId < mainViews.Count; viewId++) {
                if (mainViews[viewId] is IDontDestroyOnLoad) {
                    UnityObject.DontDestroyOnLoad(mainViews[viewId].gameObject);
                }
            }
        }
        
        internal void CheckAndAdd<T>(List<T> list) {
            for (int viewId = 0; viewId < mainViews.Count; viewId++) {
                if (mainViews[viewId] is T view) {
                    list.Add(view);
                }
            }
        }
        
        internal void Insert<T>(T view) where T : View {
            if (_isUsedViewResolve) {
                string label = view.gameObject != null ? view.gameObject.name : typeof(T).Name;
                Debug.LogError($"ViewsContext.Add({label}) - Can't be added, resolve is completed!");
                return;
            }
            
            if (view is IInit init) {
                init.Init();
            }
            
            mainViews.Add(view);
        }
        
        internal void Connect(View view, Action<ILoop> connectLoop) {
            if (view is IInit init) {
                init.Init();
            }
            
            if (view is IApplyResolving apply) {
                apply.ApplyResolving();
            }
            
            if (view is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }
            
            if (view is ILoop loop) {
                connectLoop(loop);
            }
            
            subViews.Add(view);
        }
        
        internal void Disconnect(View view, Action<ILoop> disconnectLoop) {
            if (view is ILoop loop) {
                disconnectLoop(loop);
            }
            
            if (view is IUnload unload) {
                unload.Unload();
            }
            
            subViews.Remove(view);
        }
        
        public bool TryGetGenerated<T>(out T view) where T : View, IGeneratedContext {
            for (int i = 0; i < _generated.Length; i++) {
                if (_generated[i] is T result) {
                    view = result;
                    return true;
                }
            }
            
            view = null;
            return false;
        }
        
    #if UNITY_EDITOR
        
        public void Reset() {
            List<View> views = UnityObject.FindObjectsOfType<View>(true).ToList();
            List<View> generated = new List<View>();
            
            views.Sort(CompareViewsByPriority);
            
            for (int viewId = 0; viewId < views.Count; viewId++) {
                if (views[viewId] is IGeneratedContext) {
                    generated.Add(views[viewId]);
                }
                
                if (views[viewId] is IApplyGenerated target) {
                    target.Reset();
                } else if (views[viewId] is IApplyGeneratedContext targetContext) {
                    targetContext.Reset();
                }
            }
            
            _generated = generated.ToArray();
        }
        
        [Pure]
        public int CompareViewsByPriority(View first, View second) {
            int firstPriority = first is IGeneratedPriority customFirstPriority ? customFirstPriority.priority : 0;
            int secondPriority = second is IGeneratedPriority customSecondPriority ? customSecondPriority.priority : 0;
            return secondPriority - firstPriority;
        }
        
    #endif
    }
}