using UnityEngine;

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Views {
    [DisallowMultipleComponent]
    public abstract class View : MonoBehaviour, IView {
    #if UNITY_EDITOR && ODIN_INSPECTOR
        
        [Button("Generate", DirtyOnClick = true), PropertyOrder(1000)]
        public virtual void Generate_Editor() { }
        
    #endif
    }
}