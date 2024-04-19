using System;
using System.Collections.Generic;

#if ODIN_SERIALIZATION
using Sirenix.Serialization;
#endif

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    [Serializable]
    public sealed class ObservedDisposable<T> : IDisposable where T : IDisposable {
        public T value => _value;

        private List<Action<T>> _listeners;

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, HideLabel, OnValueChanged("@" + nameof(Set) + "(" + nameof(_value) + ")")]
    #endif
    #if ODIN_SERIALIZATION
        [OdinSerialize]
    #else
        [UnityEngine.SerializeField]
    #endif
        private T _value;
        
    #if PERFORMANCE_DEBUG
        private uint _frameId;
    #endif

        public ObservedDisposable(T value) : this() => _value = value;

        public ObservedDisposable() => _listeners = new List<Action<T>>();

        public void Set(T newValue) {
            if (_value != null) {
                _value.Dispose();
            }
            
            _value = newValue;
            
            for (int i = _listeners.Count - 1; i >= 0; i--) {
                _listeners[i].Invoke(newValue);
            }
            
        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"ObservedDisposable {type.Name} in {type.Namespace} called twice in one frame!");
            }

            _frameId = ObservedUtility.frameId;
        #endif
        }

        public void AddListener(Action<T> listener) => _listeners.Add(listener);

        public void RemoveListener(Action<T> listener) => _listeners.Remove(listener);

        public void Dispose() {
            if (_value != null) {
                _value.Dispose();
            }

            _listeners = null;
        }

        public static implicit operator T(ObservedDisposable<T> value) => value._value;
    }
}