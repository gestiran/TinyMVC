using Sirenix.OdinInspector;
using TinyMVC.Views;

namespace TinyMVC.Dependencies.Components {
    public abstract class Actor<T> : Actor where T : View {
    #if UNITY_EDITOR
        internal override bool isVisibleBaseView => false;
    #endif
        
        [ShowInInspector, ReadOnly]
        public new T view {
            get => viewTypeInternal;
            internal set => viewTypeInternal = value;
        }
        
        internal override View viewInternal { get => viewTypeInternal; set => viewTypeInternal = value as T; }
        
        [ShowInInspector, LabelText("View"), ReadOnly]
        internal T viewTypeInternal { get; set; }
    }
    
    public abstract class Actor : Model {
    #if UNITY_EDITOR
        
        internal virtual bool isVisibleBaseView => true;
        
    #endif
        
        public View view {
            get => viewInternal;
            internal set => viewInternal = value;
        }
        
        [ShowInInspector, LabelText("View"), ShowIf("isVisibleBaseView"), ReadOnly]
        internal virtual View viewInternal { get; set; }
    }
}