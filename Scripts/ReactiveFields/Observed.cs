using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;
using TinyMVC.Utilities;

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
        public bool isDirty => _frameId == TimelineUtility.frameId;

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
        private uint _frameId;

        public Observed(T value) : this() => _value = value;

        public Observed() => listeners = new List<Listener<T>>();

        public void SetSilent([NotNull] T newValue) {
            _value = newValue;
            
        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"Observed {type.Name} in {type.Namespace} called twice in one frame!");
            }
        #endif
            _frameId = TimelineUtility.frameId;
        }
        
        public void Set([NotNull] T newValue) {
            _value = newValue;
            listeners.Invoke(listener => listener.Invoke(newValue));

        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"Observed {type.Name} in {type.Namespace} called twice in one frame!");
            }
        #endif
            _frameId = TimelineUtility.frameId;
        }

        public void SetNull() {
            _value = default;
            listeners.Invoke(listener => listener.Invoke(_value));

        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"Observed {type.Name} in {type.Namespace} called twice in one frame!");
            }
        #endif
            _frameId = TimelineUtility.frameId;
        }
        
        public void Unload() => listeners.Clear();

        public static implicit operator T(Observed<T> value) => value._value;

        public override string ToString() => $"Observed({typeof(T).Name}: {_value}, listeners: {listeners.Count})";
    }
}