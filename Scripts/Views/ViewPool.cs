// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyMVC.Views.Extensions;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Views {
    public abstract class ViewPool : View, IEnumerable<View> {
        public int length => views.Length;
        
        public View[] views { get => viewsInternal; set => viewsInternal = value; }
        
    #if ODIN_INSPECTOR
        [field: ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false, HideRemoveButton = true, OnTitleBarGUI = "ListGUI")]
        [field: LabelText("Views"), LabelWidth(25), ShowIf("isVisibleView"), OnValueChanged("OnViewsChangedInternal")]
    #endif
        [field: SerializeField]
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
        
    #if ODIN_INSPECTOR
        // ReSharper disable once UnusedMember.Local
        internal void ListGUI() {
            if (Sirenix.Utilities.Editor.SirenixEditorGUI.ToolbarButton(Sirenix.Utilities.Editor.EditorIcons.Refresh)) {
                UpdateViews();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
    #endif
        
        internal virtual void UpdateViews() {
            SetViews(GetComponentsInChildren<View>(true));
            OnViewsChangedInternal();
        }
        
        private void OnViewsChangedInternal() {
            if (isVisibleView) {
                OnViewsChanged();
            }
        }
        
    #endif
    }
    
    public abstract class ViewPool<T> : ViewPool, IEnumerable<T> where T : View {
        public new int length => views.Length;
        
    #if ODIN_INSPECTOR
        [field: ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false, HideRemoveButton = true, OnTitleBarGUI = "ListGUI")]
        [field: LabelWidth(25), OnValueChanged("OnViewsChangedInternalType")]
    #endif
        [field: SerializeField]
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
            SetViews(GetComponentsInChildren<T>(true));
            OnViewsChanged();
        }
        
    #if ODIN_INSPECTOR
        private void OnViewsChangedInternalType() => OnViewsChanged();
    #endif
        
    #endif
    }
}