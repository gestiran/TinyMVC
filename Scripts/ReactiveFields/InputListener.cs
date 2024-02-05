using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TinyMVC.Loop;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener : IUnload {
        private List<Action> _listeners = new List<Action>();
        
    #if UNITY_EDITOR
        private uint _frameId;
    #endif
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send() {
            for (int i = 0; i < _listeners.Count; i++) {
                _listeners[i].Invoke();
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)} called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }

        public void AddListener(Action listener) => _listeners.Add(listener);

        public void RemoveListener(Action listener) => _listeners.Remove(listener);

        public void Unload() => _listeners = null;
    }

#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T> : IUnload {
        private List<MultipleListener<T>> _listeners = new List<MultipleListener<T>>();

    #if UNITY_EDITOR
        private uint _frameId;
    #endif
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send([NotNull] params T[] data) {
            for (int i = 0; i < _listeners.Count; i++) {
                _listeners[i].Invoke(data);
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T).Name}> called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }

        public void AddListener(MultipleListener<T> listener) => _listeners.Add(listener);

        public void RemoveListener(MultipleListener<T> listener) => _listeners.Remove(listener);

        public void Unload() => _listeners = null;
    }

#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2> : IUnload {
        private List<Action<T1, T2>> _listeners = new List<Action<T1, T2>>();

    #if UNITY_EDITOR
        private uint _frameId;
    #endif
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 data1, T2 data2) {
            for (int i = 0; i < _listeners.Count; i++) {
                _listeners[i].Invoke(data1, data2);
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T1).Name}, {typeof(T2).Name}> called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }

        public void AddListener(Action<T1, T2> listener) => _listeners.Add(listener);

        public void RemoveListener(Action<T1, T2> listener) => _listeners.Remove(listener);

        public void Unload() => _listeners = null;
    }
    
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2, T3> : IUnload {
        private List<Action<T1, T2, T3>> _listeners = new List<Action<T1, T2, T3>>();

    #if UNITY_EDITOR
        private uint _frameId;
    #endif
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 data1, T2 data2, T3 data3) {
            for (int i = 0; i < _listeners.Count; i++) {
                _listeners[i].Invoke(data1, data2, data3);
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}> called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }

        public void AddListener(Action<T1, T2, T3> listener) => _listeners.Add(listener);

        public void RemoveListener(Action<T1, T2, T3> listener) => _listeners.Remove(listener);

        public void Unload() => _listeners = null;
    }
}