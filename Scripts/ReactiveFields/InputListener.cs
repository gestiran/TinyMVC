using System;
using System.Collections.Generic;
using TinyMVC.Loop;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener : IUnload {
        private readonly List<Action> _listeners;

        private const int _CAPACITY = 16;
        
        public InputListener() => _listeners = new List<Action>(_CAPACITY);

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send() {
            Action listeners = null;
            
            foreach (Action listener in _listeners) {
                listeners += listener;
            }
            
            listeners?.Invoke();
        }
        
        public void AddListener(Action listener) => _listeners.Add(listener);

        public void AddListener(Action listener, UnloadPool unload) {
            _listeners.Add(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }

        public void RemoveListener(Action listener) => _listeners.Remove(listener);

        public void Unload() => _listeners.Clear();
    }

#if ODIN_INSPECTOR && UNITY_EDITOR
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
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send(T data = default) {
            Action listeners = null;
            Action<T> valueListeners = null;
            
            foreach (Action listener in _listeners) {
                listeners += listener;
            }
            
            foreach (Action<T> listener in _valueListeners) {
                valueListeners += listener;
            }
            
            listeners?.Invoke();
            valueListeners?.Invoke(data);
        }
        
        public void Send(params T[] data) {
            Action listeners = null;
            Action<T> valueListeners = null;
            
            foreach (Action listener in _listeners) {
                listeners += listener;
            }
            
            foreach (Action<T> listener in _valueListeners) {
                valueListeners += listener;
            }
            
            listeners?.Invoke();

            for (int i = 0; i < data.Length; i++) {
                valueListeners?.Invoke(data[i]);
            }
        }
        
        public void AddListener(Action listener) => _listeners.Add(listener);

        public void AddListener(Action listener, UnloadPool unload) {
            _listeners.Add(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        public void AddListener(Action<T> listener) => _valueListeners.Add(listener);

        public void AddListener(Action<T> listener, UnloadPool unload) {
            _valueListeners.Add(listener);
            unload.Add(new UnloadAction(() => _valueListeners.Remove(listener)));
        }
        
        public void RemoveListener(Action listener) => _listeners.Remove(listener);

        public void RemoveListener(Action<T> listener) => _valueListeners.Remove(listener);

        public void Unload() {
            _listeners.Clear();
            _valueListeners.Clear();
        }
    }

#if ODIN_INSPECTOR && UNITY_EDITOR
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
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 data1 = default, T2 data2 = default) {
            Action listeners = null;
            Action<T1, T2> valueListeners = null;
            
            foreach (Action listener in _listeners) {
                listeners += listener;
            }
            
            foreach (Action<T1, T2> listener in _valueListeners) {
                valueListeners += listener;
            }
            
            listeners?.Invoke();
            valueListeners?.Invoke(data1, data2);
        }
        
        public void AddListener(Action listener) => _listeners.Add(listener);

        public void AddListener(Action listener, UnloadPool unload) {
            _listeners.Add(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        public void AddListener(Action<T1, T2> listener) => _valueListeners.Add(listener);

        public void AddListener(Action<T1, T2> listener, UnloadPool unload) {
            _valueListeners.Add(listener);
            unload.Add(new UnloadAction(() => _valueListeners.Remove(listener)));
        }
        
        public void RemoveListener(Action listener) => _listeners.Remove(listener);

        public void RemoveListener(Action<T1, T2> listener) => _valueListeners.Remove(listener);
        
        public void Unload() {
            _listeners.Clear();
            _valueListeners.Clear();
        }
    }
    
#if ODIN_INSPECTOR && UNITY_EDITOR
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
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 data1 = default, T2 data2 = default, T3 data3 = default) {
            Action listeners = null;
            Action<T1, T2, T3> valueListeners = null;
            
            foreach (Action listener in _listeners) {
                listeners += listener;
            }
            
            foreach (Action<T1, T2, T3> listener in _valueListeners) {
                valueListeners += listener;
            }
            
            listeners?.Invoke();
            valueListeners?.Invoke(data1, data2, data3);
        }
        
        public void AddListener(Action listener) => _listeners.Add(listener);

        public void AddListener(Action listener, UnloadPool unload) {
            _listeners.Add(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        public void AddListener(Action<T1, T2, T3> listener) => _valueListeners.Add(listener);

        public void AddListener(Action<T1, T2, T3> listener, UnloadPool unload) {
            _valueListeners.Add(listener);
            unload.Add(new UnloadAction(() => _valueListeners.Remove(listener)));
        }
        
        public void RemoveListener(Action listener) => _listeners.Remove(listener);

        public void RemoveListener(Action<T1, T2, T3> listener) => _valueListeners.Remove(listener);

        public void Unload() {
            _listeners.Clear();
            _valueListeners.Clear();
        }
    }
}