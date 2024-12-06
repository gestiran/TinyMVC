using System;
using System.Collections.Generic;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener : IUnload {
        private readonly List<Action> _listeners;
        
        private const int _CAPACITY = 16;
        
        public InputListener() => _listeners = new List<Action>(_CAPACITY);
        
        public InputListener(Action action) : this() => AddListener(action);
        
        public InputListener(Action action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send() => _listeners.Invoke();
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action listener, UnloadPool unload) {
            _listeners.Add(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(Action listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => _listeners.Clear();
    }
    
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T> : IUnload {
        private readonly List<Action> _listeners;
        private readonly List<Action<T>> _valueListeners;
        
        private const int _CAPACITY = 16;
        
        public InputListener() {
            _listeners = new List<Action>(_CAPACITY);
            _valueListeners = new List<Action<T>>(_CAPACITY);
        }
        
        public InputListener(Action action) : this() => AddListener(action);
        
        public InputListener(Action<T> action) : this() => AddListener(action);
        
        public InputListener(Action action, UnloadPool unload) : this() => AddListener(action, unload);
        
        public InputListener(Action<T> action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send(T data = default) {
            _listeners.Invoke();
            _valueListeners.Invoke(data);
        }
        
        public void Send(params T[] data) {
            _listeners.Invoke();
            _valueListeners.Invoke(data);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action listener, UnloadPool unload) {
            _listeners.Add(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action<T> listener) => _valueListeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action<T> listener, UnloadPool unload) {
            _valueListeners.Add(listener);
            unload.Add(new UnloadAction(() => _valueListeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(Action listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(Action<T> listener) => _valueListeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _valueListeners.Clear();
        }
    }
    
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2> : IUnload {
        private readonly List<Action> _listeners;
        private readonly List<Action<T1, T2>> _valueListeners;
        
        private const int _CAPACITY = 16;
        
        public InputListener() {
            _listeners = new List<Action>(_CAPACITY);
            _valueListeners = new List<Action<T1, T2>>(_CAPACITY);
        }
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 data1 = default, T2 data2 = default) {
            _listeners.Invoke();
            _valueListeners.Invoke(data1, data2);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action listener, UnloadPool unload) {
            _listeners.Add(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action<T1, T2> listener) => _valueListeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action<T1, T2> listener, UnloadPool unload) {
            _valueListeners.Add(listener);
            unload.Add(new UnloadAction(() => _valueListeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(Action listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(Action<T1, T2> listener) => _valueListeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _valueListeners.Clear();
        }
    }
    
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2, T3> : IUnload {
        private readonly List<Action> _listeners;
        private readonly List<Action<T1, T2, T3>> _valueListeners;
        
        private const int _CAPACITY = 16;
        
        public InputListener() {
            _listeners = new List<Action>(_CAPACITY);
            _valueListeners = new List<Action<T1, T2, T3>>(_CAPACITY);
        }
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 data1 = default, T2 data2 = default, T3 data3 = default) {
            _listeners.Invoke();
            _valueListeners.Invoke(data1, data2, data3);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action listener, UnloadPool unload) {
            _listeners.Add(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action<T1, T2, T3> listener) => _valueListeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Action<T1, T2, T3> listener, UnloadPool unload) {
            _valueListeners.Add(listener);
            unload.Add(new UnloadAction(() => _valueListeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(Action listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(Action<T1, T2, T3> listener) => _valueListeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _valueListeners.Clear();
        }
    }
}