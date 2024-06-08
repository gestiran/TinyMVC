using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Views {
    public abstract class ViewPool<T> : View where T : IView {
        public int length => views.Length;
        
        #if ODIN_INSPECTOR && UNITY_EDITOR
        [field: SerializeField, OnValueChanged(nameof(OnViewsChanged))]
        #else
        [field: SerializeField]
        #endif
        public T[] views { get; private set; }
        
        public T this[int index] => views[index];
        
        #if UNITY_EDITOR
        
        public virtual void Reset() => views = GetComponentsInChildren<T>(true);
        
        #if ODIN_INSPECTOR
        protected virtual void OnViewsChanged() { }
        #endif
        #endif
    }
}