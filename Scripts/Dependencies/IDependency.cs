using System;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies {
    #if UNITY_EDITOR
    [HideReferenceObjectPicker, HideDuplicateReferenceBox]
    #endif
    public interface IDependency {
        public Type GetType();
    }
}