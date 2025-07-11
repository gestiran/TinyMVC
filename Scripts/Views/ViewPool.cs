using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TinyMVC.Views {
    public abstract class ViewPool<T> : View, IEnumerable<T> {
        public int length => views.Length;
        
        [field: SerializeField, LabelWidth(25), OnValueChanged("OnViewsChanged")]
        [field: ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false, HideRemoveButton = true, OnTitleBarGUI = "ListGUI")]
        public T[] views { get; private set; }
        
        public T this[int index] => views[index];
        
        public IEnumerator<T> GetEnumerator() {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                yield return views[viewId];
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
    #if UNITY_EDITOR
        
        public override void Reset() {
            UpdateViews();
            base.Reset();
        }
        
        protected virtual void OnViewsChanged() { }
        
        // ReSharper disable once UnusedMember.Local
        private void ListGUI() {
            if (Sirenix.Utilities.Editor.SirenixEditorGUI.ToolbarButton(Sirenix.Utilities.Editor.EditorIcons.Refresh)) {
                UpdateViews();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
        
        private void UpdateViews() => views = GetComponentsInChildren<T>(true);
        
    #endif
    }
}