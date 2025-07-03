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
        private readonly int _id;
        private readonly Dictionary<int, ActionListener> _listeners;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new Dictionary<int, ActionListener>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send() => _listeners.Invoke();
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener.GetHashCode(), listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            int hash = listener.GetHashCode();
            _listeners.Add(hash, listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(hash)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener.GetHashCode());
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => _listeners.Clear();
        
        public override int GetHashCode() => _id;
    }
    
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T> : IUnload {
        private readonly int _id;
        private readonly Dictionary<int, ActionListener> _listeners;
        private readonly Dictionary<int, ActionListener<T>> _listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new Dictionary<int, ActionListener>(capacity);
            _listenersValue = new Dictionary<int, ActionListener<T>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T> action, UnloadPool unload) : this() => AddListener(action, unload);
        
        [Obsolete("Can't use without parameters!", true)]
        public void Send() {
            // Do nothing
        }
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send(T data) {
            _listeners.Invoke();
            _listenersValue.Invoke(data);
        }
        
        public void Send(params T[] data) {
            _listeners.Invoke();
            _listenersValue.Invoke(data);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener.GetHashCode(), listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            int hash = listener.GetHashCode();
            _listeners.Add(hash, listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(hash)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener) => _listenersValue.Add(listener.GetHashCode(), listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener, UnloadPool unload) {
            int hash = listener.GetHashCode();
            _listenersValue.Add(hash, listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(hash)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener.GetHashCode());
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T> listener) => _listenersValue.Remove(listener.GetHashCode());
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
    
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2> : IUnload {
        private readonly int _id;
        private readonly Dictionary<int, ActionListener> _listeners;
        private readonly Dictionary<int, ActionListener<T1, T2>> _listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new Dictionary<int, ActionListener>(capacity);
            _listenersValue = new Dictionary<int, ActionListener<T1, T2>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T1, T2> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2> action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 data1, T2 data2) {
            _listeners.Invoke();
            _listenersValue.Invoke(data1, data2);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener.GetHashCode(), listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            int hash = listener.GetHashCode();
            _listeners.Add(hash, listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(hash)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2> listener) => _listenersValue.Add(listener.GetHashCode(), listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2> listener, UnloadPool unload) {
            int hash = listener.GetHashCode();
            _listenersValue.Add(hash, listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(hash)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener.GetHashCode());
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T1, T2> listener) => _listenersValue.Remove(listener.GetHashCode());
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
    
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2, T3> : IUnload {
        private readonly int _id;
        private readonly Dictionary<int, ActionListener> _listeners;
        private readonly Dictionary<int, ActionListener<T1, T2, T3>> _listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new Dictionary<int, ActionListener>(capacity);
            _listenersValue = new Dictionary<int, ActionListener<T1, T2, T3>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T1, T2, T3> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2, T3> action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 data1, T2 data2, T3 data3) {
            _listeners.Invoke();
            _listenersValue.Invoke(data1, data2, data3);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener.GetHashCode(), listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            int hash = listener.GetHashCode();
            _listeners.Add(hash, listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(hash)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2, T3> listener) => _listenersValue.Add(listener.GetHashCode(), listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2, T3> listener, UnloadPool unload) {
            int hash = listener.GetHashCode();
            _listenersValue.Add(hash, listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(hash)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener.GetHashCode());
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T1, T2, T3> listener) => _listenersValue.Remove(listener.GetHashCode());
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
}