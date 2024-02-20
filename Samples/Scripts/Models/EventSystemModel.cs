using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;

namespace TinyMVC.Samples.Models {
    public sealed class EventSystemModel : IDependency {
        public Observed<bool> isActive;
    }
}