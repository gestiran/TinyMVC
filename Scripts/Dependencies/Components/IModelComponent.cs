using System;

using Sirenix.OdinInspector;
#if ODIN_INSPECTOR && UNITY_EDITOR
#endif

namespace TinyMVC.Dependencies.Components {
    /// <summary> Marker of an object marking it as a possible dependency </summary>
#if ODIN_INSPECTOR && UNITY_EDITOR
    [HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public interface IModelComponent {
        public Type GetType();
    }
}