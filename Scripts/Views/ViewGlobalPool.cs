using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TinyMVC.Views.Extensions;
using TinyUtilities.Unity;

namespace TinyMVC.Views {
    public abstract class ViewPoolGlobal : View, IEnumerable<View> {
        public int length => views.Length;
        
        public View[] views { get => viewsInternal; set => viewsInternal = value; }
        
        [field: SerializeField, LabelText("Views"), LabelWidth(25), ShowIf("isVisibleView"), OnValueChanged("OnViewsChangedInternal")]
        [field: ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false, HideRemoveButton = true, OnTitleBarGUI = "ListGUI")]
        internal virtual View[] viewsInternal { get; set; }
        
        public View this[int index] => viewsInternal[index];
        
        public void SetViews(View[] value) => viewsInternal = value;
        
        public IEnumerator<View> GetEnumerator() {
            for (int viewId = 0; viewId < viewsInternal.Length; viewId++) {
                yield return viewsInternal[viewId];
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public abstract Type GetTypeView();
        
    #if UNITY_EDITOR
        
        internal virtual bool isVisibleView => true;
        
        public override void Reset() {
            UpdateViews();
            base.Reset();
        }
        
        protected virtual void OnViewsChanged() { }
        
        // ReSharper disable once UnusedMember.Local
        internal void ListGUI() {
            if (Sirenix.Utilities.Editor.SirenixEditorGUI.ToolbarButton(Sirenix.Utilities.Editor.EditorIcons.Refresh)) {
                UpdateViews();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
        
        internal virtual void UpdateViews() {
            if (GameObjectUtility.TryFindObjectsOfTypePrefab(out View[] result)) {
                SetViews(result);
            } else {
                SetViews(FindObjectsOfType<View>(true));
            }
            
            OnViewsChanged();
        }
        
        private void OnViewsChangedInternal() {
            if (isVisibleView) {
                OnViewsChanged();
            }
        }
        
    #endif
    }
    
    public abstract class ViewPoolGlobal<T> : ViewPoolGlobal, IEnumerable<T> where T : View {
        public new int length => views.Length;
        
        [field: SerializeField, LabelWidth(25), OnValueChanged("OnViewsChangedInternalType")]
        [field: ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false, HideRemoveButton = true, OnTitleBarGUI = "ListGUI")]
        public new T[] views { get; private set; }
        
        private static readonly Type _viewType = typeof(T);
        
        internal override View[] viewsInternal { get => views.AsBaseView(); set => views = value.AsTargetView<T>(); }
        
        public new T this[int index] => views[index];
        
        public void SetViews(T[] value) => views = value;
        
        public new IEnumerator<T> GetEnumerator() {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                yield return views[viewId];
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public override Type GetTypeView() => _viewType;
        
    #if UNITY_EDITOR
        
        internal override bool isVisibleView => false;
        
        protected new virtual void OnViewsChanged() { }
        
        internal override void UpdateViews() {
            if (GameObjectUtility.TryFindObjectsOfTypePrefab(out T[] result)) {
                SetViews(result);   
            } else {
                SetViews(FindObjectsOfType<T>(true));
            }
            
            OnViewsChanged();
        }
        
        private void OnViewsChangedInternalType() => OnViewsChanged();
        
    #endif
    }
}