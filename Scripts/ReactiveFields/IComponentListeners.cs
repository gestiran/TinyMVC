using TinyMVC.Dependencies.Components;

namespace TinyMVC.ReactiveFields {
    internal interface IComponentListeners {
        public void TryInvokeAdd(Model model, ModelComponent component);
        
        public void TryInvokeRemove(Model model, ModelComponent component);
    }
}