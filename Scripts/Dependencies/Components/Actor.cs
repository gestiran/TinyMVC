using Sirenix.OdinInspector;
using TinyMVC.Views;

namespace TinyMVC.Dependencies.Components {
    public abstract class Actor<T> : Actor where T : View {
        [field: ShowInInspector, LabelText("View"), HideReferenceObjectPicker, HideDuplicateReferenceBox, PropertyOrder(-10000), ReadOnly]
        public new T view { get; internal set; }
        
        internal override View viewInternal { get => view; set => view = value as T; }
        
    #if UNITY_EDITOR
        
        internal override bool isVisibleView => false;
        
    #endif
    }
    
    public abstract class Actor : Model {
        public View view { get => viewInternal; internal set => viewInternal = value; }
        
        [field: ShowInInspector, LabelText("View"), HideReferenceObjectPicker, HideDuplicateReferenceBox, PropertyOrder(-10000), ShowIf(nameof(isVisibleView)), ReadOnly]
        internal virtual View viewInternal { get; set; }
        
    #if UNITY_EDITOR
        
        internal virtual bool isVisibleView => true;
        
    #endif
    }
}