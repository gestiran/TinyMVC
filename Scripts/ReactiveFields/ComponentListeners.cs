// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Dependencies.Components;
using TinyReactive.Fields;

namespace TinyMVC.ReactiveFields {
    internal sealed class ComponentListeners<TModel, TComponent> : IComponentListeners where TModel : Model where TComponent : ModelComponent {
        private readonly LazyList<ActionListener<TModel, TComponent>> _addListeners;
        private readonly LazyList<ActionListener<TModel, TComponent>> _removeListeners;
        
        private const int _CAPACITY = 8;
        
        public ComponentListeners() {
            _addListeners = new LazyList<ActionListener<TModel, TComponent>>(_CAPACITY);
            _removeListeners = new LazyList<ActionListener<TModel, TComponent>>(_CAPACITY);
        }
        
        public void AddOnAddListener(ActionListener<TModel, TComponent> listener) => _addListeners.Add(listener);
        
        public void RemoveOnAddListener(ActionListener<TModel, TComponent> listener) => _addListeners.Remove(listener);
        
        public void AddOnRemoveListener(ActionListener<TModel, TComponent> listener) => _removeListeners.Add(listener);
        
        public void RemoveOnRemoveListener(ActionListener<TModel, TComponent> listener) => _removeListeners.Remove(listener);
        
        public void TryInvokeAdd(Model model, ModelComponent component) {
            if (model is TModel targetModel && component is TComponent targetComponent) {
                if (_addListeners.isDirty) {
                    _addListeners.Apply();
                }
                
                for (int i = 0; i < _addListeners.count; i++) {
                    _addListeners[i].Invoke(targetModel, targetComponent);
                }
            }
        }
        
        public void TryInvokeRemove(Model model, ModelComponent component) {
            if (model is TModel targetModel && component is TComponent targetComponent) {
                if (_removeListeners.isDirty) {
                    _removeListeners.Apply();
                }
                
                for (int i = 0; i < _removeListeners.count; i++) {
                    _removeListeners[i].Invoke(targetModel, targetComponent);
                }
            }
        }
    }
}