using System;
using System.Collections.Generic;
using TinyMVC.Loop;

#if ODIN_SERIALIZATION
using Sirenix.Serialization;
#endif

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
#endif

namespace TinyMVC.ReactiveFields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    [Serializable]
    public sealed class Observed<T> : IUnload {
        public T value => _value;

        private List<Action<T>> _listeners;

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, HideLabel, OnValueChanged("@" + nameof(Set) + "(" + nameof(_value) + ")"), HideDuplicateReferenceBox, HideReferenceObjectPicker]
    #endif
    #if ODIN_SERIALIZATION
        [OdinSerialize]
    #else
        [UnityEngine.SerializeField]
    #endif
        private T _value;

    #if UNITY_EDITOR
        private uint _frameId;
    #endif

        public Observed(T value) : this() => _value = value;

        public Observed() => _listeners = new List<Action<T>>();

        public void Set(T newValue) {
            _value = newValue;
            
            for (int i = _listeners.Count - 1; i >= 0; i--) {
                _listeners[i].Invoke(newValue);
            }

        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"Observed {type.Name} in {type.Namespace} called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }
        
        public void AddListener(Action<T> listener) => _listeners.Add(listener);

        public void RemoveListener(Action<T> listener) => _listeners.Remove(listener);

        public void Unload() => _listeners.Clear();

        public static implicit operator T(Observed<T> value) => value._value;

        public override string ToString() => $"Observed({typeof(T).Name}: {_value}, listeners: {_listeners.Count})";
    }
}