using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TinyMVC.Loop;

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
    public sealed class Observed<T> : IUnload {
        public T value => _value;

        internal List<Listener<T>> listeners;

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

        public Observed() => listeners = new List<Listener<T>>();

        public void Set([NotNull] T newValue) {
            _value = newValue;
            
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].Invoke(newValue);
            }

        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"Observed {type.Name} in {type.Namespace} called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }
        
        public void SetNull() {
            _value = default;
            
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].InvokeNull();
            }

        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"Observed {type.Name} in {type.Namespace} called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }
        
        public void Unload() => listeners.Clear();

        public static implicit operator T(Observed<T> value) => value._value;

        public override string ToString() => $"Observed({typeof(T).Name}: {_value}, listeners: {listeners.Count})";
    }
}