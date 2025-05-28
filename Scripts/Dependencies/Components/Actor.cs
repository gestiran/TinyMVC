using TinyMVC.Views;

namespace TinyMVC.Dependencies.Components {
    public abstract class Actor<T> : Actor where T : View {
        public new T view { get; internal set; }
    }
    
    public abstract class Actor : Model {
        public View view { get; internal set; }
    }
}