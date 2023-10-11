using System;
using System.Collections.Generic;
using TinyMVC.Utilities.Updating;
using TinyMVC.Views;
using TinyDI.Dependencies.Models;
using TinyDI.Dependencies.Parameters;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
#if ODIN_INSPECTOR
    [InlineProperty, HideLabel]
#endif
    [Serializable]
    public abstract class BootViews : IDisposable {
        protected List<IView> _views;

        public abstract void Instantiate();

        public virtual void Create() {
            _views = new List<IView>();
        }

        public void Init() {
            for (int viewId = 0; viewId < _views.Count; viewId++) {
                if (_views[viewId] is IInitView view) {
                    view.Init();
                }
            }
        }

        public void GetParametersResolvers(List<IParametersResolving> resolving) {
            for (int viewId = 0; viewId < _views.Count; viewId++) {
                if (_views[viewId] is IParametersResolving view) {
                    resolving.Add(view);
                }
            }
        }

        public void GetModelsResolvers(List<IModelsResolving> resolving) {
            for (int viewId = 0; viewId < _views.Count; viewId++) {
                if (_views[viewId] is IModelsResolving view) {
                    resolving.Add(view);
                }
            }
        }

        public void StartView() {
            for (int viewId = 0; viewId < _views.Count; viewId++) {
                if (_views[viewId] is IStartView view) {
                    view.StartView();
                }
            }
        }

        public void StartUpdateLoop() {
            for (int viewId = 0; viewId < _views.Count; viewId++) {
                UpdateLoopUtility.TryAddSystem(_views[viewId]);
            }
        }

        public void Dispose() {
            for (int viewId = 0; viewId < _views.Count; viewId++) {
                if (_views[viewId] is IDisposable view) {
                    view.Dispose();
                }
            }
        }

    #if UNITY_EDITOR

    #if ODIN_INSPECTOR
        [Button("Generate", DirtyOnClick = true), HideInPlayMode, ShowIn(PrefabKind.InstanceInScene)]
    #endif
        public virtual void Generate_Editor() { }

    #endif
    }
}