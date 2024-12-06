using UnityEngine;

namespace TinyMVC.Views {
    public abstract class ViewComponent : MonoBehaviour {
    #if UNITY_EDITOR
        
        public virtual void Reset() => UnityEditor.EditorUtility.SetDirty(this);
        
    #endif
    }
}