using System;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies {
    /// <summary> Marker of an object marking it as a possible dependency </summary>
    #if ODIN_INSPECTOR && UNITY_EDITOR
    [HideReferenceObjectPicker, HideDuplicateReferenceBox]
    #endif
    public interface IDependency {
        public Type GetType();
    }
}