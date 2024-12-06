using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Samples.Views.Global;

namespace TinyMVC.Samples.Binding.Global {
    public sealed class EventSystemBind : Binder<EventSystemModel> {
        [Inject] private EventSystemView _view;

        protected override void Bind(EventSystemModel model) {
            model.isActive = new Observed<bool>(_view.isActive);
        }
    }
}