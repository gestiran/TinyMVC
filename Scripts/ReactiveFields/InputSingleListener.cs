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
    public sealed class InputSingleListener<T> : IUnload {
        private List<Action<T>> _listeners = new List<Action<T>>();

    #if UNITY_EDITOR
        private uint _frameId;
    #endif

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send(T data) {
            for (int i = _listeners.Count - 1; i >= 0; i--) {
                _listeners[i].Invoke(data);
            }

        #if UNITY_EDITOR
                if (_frameId == ObservedTestUtility.frameId) {
                    UnityEngine.Debug.LogWarning($"InputSingleListener<{typeof(T).Name}> called twice in one frame!");
                }
                
                _frameId = ObservedTestUtility.frameId;
        #endif
        }

        public void AddListener(Action<T> listener) => _listeners.Add(listener);

        public void RemoveListener(Action<T> listener) => _listeners.Remove(listener);

        public void Unload() => _listeners = null;
    }
}