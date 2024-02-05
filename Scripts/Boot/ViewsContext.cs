using System;
using System.Collections.Generic;
using TinyMVC.Extensions;
using TinyMVC.Views;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

using UnityObject = UnityEngine.Object;

namespace TinyMVC.Boot {
    /// <summary> Contains references to scene objects </summary>
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideLabel]
#endif
    [Serializable]
    public abstract class ViewsContext {
        private List<IView> _views;
        
        internal void Create() {
            _views = new List<IView>();
            Create(_views);
        }

        internal void Init() => _views.TryInit();

        internal void BeginPlay() => _views.TryBeginPlay();
        
        internal void CheckAndAdd<T>(List<T> list) {
            list.Capacity += list.Count;
            
            for (int viewId = 0; viewId < _views.Count; viewId++) {
                if (_views[viewId] is T view) {
                    list.Add(view);
                }
            }
        }

        internal void ApplyDontDestroyOnLoad() {
            for (int viewId = 0; viewId < _views.Count; viewId++) {
                if (_views[viewId] is IDontDestroyOnLoad && _views[viewId] is MonoBehaviour monoView) {
                    UnityObject.DontDestroyOnLoad(monoView);
                }
            }
        }
        
        internal void Unload() => _views.TryUnload();
        
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