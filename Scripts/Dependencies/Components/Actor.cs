using Sirenix.OdinInspector;
using TinyMVC.Views;

namespace TinyMVC.Dependencies.Components {
    public abstract class Actor<T> : Actor where T : View {
        public new T view { get => _view; internal set => _view = value; }
        
        internal override View viewInternal { get => _view; set => _view = value as T; }
        
        private T _view { get; set; }
    }
    
    public abstract class Actor : Model {
        public View view { get => viewInternal; internal set => viewInternal = value; }
        
        internal virtual View viewInternal { get => _view; set => _view = value; }
        
        [ShowInInspector, LabelText("View"), HideReferenceObjectPicker, HideDuplicateReferenceBox, PropertyOrder(-10000), ReadOnly]
        private View _view;
    }
}