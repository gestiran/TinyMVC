using System;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies {
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public interface IDependency {
        public Type GetType();
    }
}