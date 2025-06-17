using System.Collections.Generic;
using TinyMVC.Dependencies.Components;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.ReactiveFields {
    internal static class ComponentListeners<T> where T : ModelComponent {
        private static readonly List<ActionListener<Model, T>> _addListeners;
        private static readonly List<ActionListener<Model, T>> _removeListeners;
        
        private const int _CAPACITY = 8;
        
        static ComponentListeners() {
            _addListeners = new List<ActionListener<Model, T>>(_CAPACITY);
            _removeListeners = new List<ActionListener<Model, T>>(_CAPACITY);
        }
        
        public static void InvokeAdd(Model model, T component) => _addListeners.Invoke(model, component);
        
        public static void InvokeRemove(Model model, T component) => _removeListeners.Invoke(model, component);
        
        public static void AddOnAddListener(ActionListener<Model, T> listener) => _addListeners.Add(listener);
        
        public static void RemoveOnAddListener(ActionListener<Model, T> listener) => _addListeners.Remove(listener);
        
        public static void AddOnRemoveListener(ActionListener<Model, T> listener) => _removeListeners.Add(listener);
        
        public static void RemoveOnRemoveListener(ActionListener<Model, T> listener) => _removeListeners.Remove(listener);
    }
}