using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.ReactiveFields {
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputSingleListener : IUnload {
        private readonly List<Func<bool>> _listeners;
        
        private const int _CAPACITY = 16;
        
        public InputSingleListener() => _listeners = new List<Func<bool>>(_CAPACITY);
        
        public InputSingleListener(Func<bool> action) : this() => AddListener(action);
        
        public InputSingleListener(Func<bool> action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send() => _listeners.Invoke();
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Func<bool> listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Func<bool> listener, UnloadPool unload) {
            _listeners.Add(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(Func<bool> listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => _listeners.Clear();
    }
}