using UnityEngine;

namespace TinyMVC.Views {
    public abstract class ViewPool<T> : View where T : IView {
        [field: SerializeField]
        public T[] views { get; private set; }

        public virtual void Reset() => views = GetComponentsInChildren<T>(true);
    }
}