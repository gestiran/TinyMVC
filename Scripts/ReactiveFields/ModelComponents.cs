using System.Collections.Generic;
using TinyMVC.Dependencies.Components;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.ReactiveFields {
    internal static class ModelComponents<TModel, TComponent> where TModel : Model where TComponent : ModelComponent {
        private static readonly List<ActionListener<TModel, TComponent>> _addListeners;
        private static readonly List<ActionListener<TModel, TComponent>> _removeListeners;
        
        private const int _CAPACITY = 8;
        
        static ModelComponents() {
            _addListeners = new List<ActionListener<TModel, TComponent>>(_CAPACITY);
            _removeListeners = new List<ActionListener<TModel, TComponent>>(_CAPACITY);
        }
        
        public static void InvokeAdd(TModel model, TComponent component) => _addListeners.Invoke(model, component);
        
        public static void InvokeRemove(TModel model, TComponent component) => _removeListeners.Invoke(model, component);
        
        public static void AddOnAddListener(ActionListener<TModel, TComponent> listener) => _addListeners.Add(listener);
        
        public static void RemoveOnAddListener(ActionListener<TModel, TComponent> listener) => _addListeners.Remove(listener);
        
        public static void AddOnRemoveListener(ActionListener<TModel, TComponent> listener) => _removeListeners.Add(listener);
        
        public static void RemoveOnRemoveListener(ActionListener<TModel, TComponent> listener) => _removeListeners.Remove(listener);
    }
}