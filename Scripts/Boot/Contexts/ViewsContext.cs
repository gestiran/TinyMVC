﻿// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
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
    public abstract class ViewsContext {
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
        
        internal void Instantiate() {
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
            
            _instances = new View[instances.Count];
            instances.CopyTo(_instances);
            _isUsedViewResolve = false;
        }
        
        internal void GetDependencies(List<IDependency> dependencies) {
            for (int assetId = 0; assetId < mainViews.Count; assetId++) {
                if (mainViews[assetId] is IDependency dependency) {
                    dependencies.Add(dependency);
                }
            }
            
            _isUsedViewResolve = true;
        }
        
        internal void CreateViews() {
            mainViews = new List<View>();
            subViews = new List<View>();
            
            Create();
            
            mainViews.AddRange(_instances);
            mainViews.AddRange(_generated);
        }
        
        internal async UniTask InitAsync() {
            for (int viewId = 0; viewId < mainViews.Count; viewId++) {
                mainViews[viewId].connectState = View.ConnectState.Connected;
            }
            
            await mainViews.TryInitAsync();
        }
        
        internal async UniTask BeginPlay() => await mainViews.TryBeginPlayAsync();
        
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
        
        internal void Connect(View view, Action<ILoop> connectLoop, Action<IResolving> resolve) {
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
        
        internal void Unload() {
            subViews.TryUnload();
            mainViews.TryUnload();
            
            subViews.Clear();
            mainViews.Clear();
        }
        
        protected void Add<T>(T view) where T : View => mainViews.Add(view);
        
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
        
        protected abstract void Create();
        
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