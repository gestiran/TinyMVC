using UnityEngine;
using Sirenix.OdinInspector;

namespace TinyMVC.Views {
    public abstract class ViewPool<T> : View {
        public int length => views.Length;
        
        [field: SerializeField, OnValueChanged("OnViewsChanged")]
        public T[] views { get; private set; }
        
        public T this[int index] => views[index];
        
    #if UNITY_EDITOR
        
        public override void Reset() {
            views = GetComponentsInChildren<T>(true);
            base.Reset();
        }
        
        protected virtual void OnViewsChanged() { }
        
    #endif
    }
}