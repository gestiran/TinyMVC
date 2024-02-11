using UnityEngine;

namespace TinyMVC.Views {
    public abstract class ViewPool<T> : View where T : IView {
        public int length => views.Length;
        
        [field: SerializeField]
        public T[] views { get; private set; }

        public T this[int index] => views[index];

        public virtual void Reset() => views = GetComponentsInChildren<T>(true);
    }
}