using TinyMVC.Dependencies.Components;
using TinyMVC.Views;

namespace TinyMVC.Boot.Binding {
    public abstract class ActorBinder<TActor> : Binder<TActor> where TActor : Actor, new() {
        protected View _view;
        
        protected ActorBinder(View view = null) => _view = view;
        
        protected ActorBinder(string key, View view = null) : base(key) => _view = view;
        
        internal override void BindInternal(TActor model) => model.viewInternal = _view;
    }
    
    public abstract class ActorBinder<TActor, TView> : ActorBinder<TActor> where TActor : Actor<TView>, new() where TView : View {
        protected new TView _view;
        
        protected ActorBinder(TView view = null) : base(view) => _view = view;
        
        protected ActorBinder(string key, TView view = null) : base(key, view) => _view = view;
        
        internal override void BindInternal(TActor model) => model.viewInternal = _view;
    }
}