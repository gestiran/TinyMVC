using TinyMVC.Dependencies.Components;
using TinyMVC.Views;

namespace TinyMVC.Boot.Binding {
    public abstract class ActorBinder<TActor> : Binder<TActor> where TActor : Actor, new() {
        protected View _view;
        
        protected ActorBinder(View view) => _view = view;
        
        internal override void BindInternal(TActor model) => model.view = _view;
    }
    
    public abstract class ActorBinder<TActor, TView> : ActorBinder<TActor> where TActor : Actor<TView>, new() where TView : View {
        protected new TView _view;
        
        protected ActorBinder(TView view) : base(view) => _view = view;
        
        internal override void BindInternal(TActor model) => model.view = _view;
    }
}