#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Controllers {
#if ODIN_INSPECTOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public interface IController { }
}