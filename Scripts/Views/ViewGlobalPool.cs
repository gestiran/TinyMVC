using Sirenix.OdinInspector;
using UnityEngine;

namespace TinyMVC.Views {
    public abstract class ViewGlobalPool<T> : View where T : Object, IView  {
        public int length => views.Length;
        
        [field: SerializeField, OnValueChanged(nameof(OnViewsChanged))]
        public T[] views { get; private set; }

        public T this[int index] => views[index];

    #if UNITY_EDITOR
        
        public virtual void Reset() => views = FindObjectsOfType<T>(true);
        
    #endif

        protected virtual void OnViewsChanged() { }
    }
}