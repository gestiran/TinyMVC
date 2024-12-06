using UnityEngine;
using Sirenix.OdinInspector;

namespace TinyMVC.Views {
    public abstract class ViewGlobalPool<T> : View where T : View {
        public int length => views.Length;
        
        [field: SerializeField, OnValueChanged("OnViewsChanged")]
        public T[] views { get; private set; }
        
        public T this[int index] => views[index];
        
    #if UNITY_EDITOR
        
        public override void Reset() {
            ApplyViews_Editor(FindObjectsOfType<T>(true));
            base.Reset();
        }
        
        protected virtual void ApplyViews_Editor(T[] value) => views = value;
        
        protected virtual void OnViewsChanged() { }
    
    #endif
    }
}