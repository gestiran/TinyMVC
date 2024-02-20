using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Models;
using TinyMVC.Samples.Views;

namespace TinyMVC.Samples.Binding {
    public sealed class EventSystemBind : Binder<EventSystemModel> {
        [Inject] private EventSystemView _view;

        protected override void Bind(EventSystemModel model) {
            model.isActive = new Observed<bool>(_view.isActive);
        }
    }
}