using System.Collections.Generic;
using TinyMVC.Dependencies.Components;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.ReactiveFields {
    internal sealed class ComponentListeners<TModel, TComponent> : IComponentListeners where TModel : Model where TComponent : ModelComponent {
        private readonly Dictionary<int, ActionListener<TModel, TComponent>> _addListeners;
        private readonly Dictionary<int, ActionListener<TModel, TComponent>> _removeListeners;
        
        private const int _CAPACITY = 8;
        
        public ComponentListeners() {
            _addListeners = new Dictionary<int, ActionListener<TModel, TComponent>>(_CAPACITY);
            _removeListeners = new Dictionary<int, ActionListener<TModel, TComponent>>(_CAPACITY);
        }
        
        public void AddOnAddListener(ActionListener<TModel, TComponent> listener) => _addListeners.Add(listener.GetHashCode(), listener);
        
        public void RemoveOnAddListener(ActionListener<TModel, TComponent> listener) => _addListeners.Remove(listener.GetHashCode());
        
        public void AddOnRemoveListener(ActionListener<TModel, TComponent> listener) => _removeListeners.Add(listener.GetHashCode(), listener);
        
        public void RemoveOnRemoveListener(ActionListener<TModel, TComponent> listener) => _removeListeners.Remove(listener.GetHashCode());
        
        public void TryInvokeAdd(Model model, ModelComponent component) {
            if (model is TModel targetModel && component is TComponent targetComponent) {
                _addListeners.Invoke(targetModel, targetComponent);
            }
        }
        
        public void TryInvokeRemove(Model model, ModelComponent component) {
            if (model is TModel targetModel && component is TComponent targetComponent) {
                _removeListeners.Invoke(targetModel, targetComponent);
            }
        }
    }
}