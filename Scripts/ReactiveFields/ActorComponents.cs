using System.Collections.Generic;
using TinyMVC.Dependencies.Components;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.ReactiveFields {
    internal static class ActorComponents<T> where T : ModelComponent {
        private static readonly List<ActionListener<Actor, T>> _addListeners;
        private static readonly List<ActionListener<Actor, T>> _removeListeners;
        
        private const int _CAPACITY = 8;
        
        static ActorComponents() {
            _addListeners = new List<ActionListener<Actor, T>>(_CAPACITY);
            _removeListeners = new List<ActionListener<Actor, T>>(_CAPACITY);
        }
        
        public static void InvokeAdd(Actor model, T component) => _addListeners.Invoke(model, component);
        
        public static void InvokeRemove(Actor model, T component) => _removeListeners.Invoke(model, component);
        
        public static void AddOnAddListener(ActionListener<Actor, T> listener) => _addListeners.Add(listener);
        
        public static void RemoveOnAddListener(ActionListener<Actor, T> listener) => _addListeners.Remove(listener);
        
        public static void AddOnRemoveListener(ActionListener<Actor, T> listener) => _removeListeners.Add(listener);
        
        public static void RemoveOnRemoveListener(ActionListener<Actor, T> listener) => _removeListeners.Remove(listener);
    }
}