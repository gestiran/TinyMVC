using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;

namespace TinyMVC.Samples.Models.Global {
    public sealed class EventSystemModel : IDependency {
        public Observed<bool> isActive;
    }
}