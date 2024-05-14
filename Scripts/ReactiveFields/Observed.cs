using System;
using System.Collections.Generic;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

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

        private List<Action> _listeners;
        private List<Action<T>> _valueListeners;

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, HideLabel, OnValueChanged("@" + nameof(Set) + "(" + nameof(_value) + ")"), HideDuplicateReferenceBox, HideReferenceObjectPicker]
    #endif
    #if ODIN_SERIALIZATION
        [OdinSerialize]
    #else
        [UnityEngine.SerializeField]
    #endif
        private T _value;

        private const int _CAPACITY = 16;

        public Observed(T value) : this() => _value = value;

        public Observed() {
            _listeners = new List<Action>(_CAPACITY);
            _valueListeners = new List<Action<T>>(_CAPACITY);
        }

        public void SetSilent(T newValue) => _value = newValue;

        public void Set(T newValue) {
            _value = newValue;
            _listeners.Invoke();
            _valueListeners.Invoke(newValue);
        }

        public void AddListener(Action listener) => _listeners.Add(listener);

        public void AddListener(Action listener, UnloadPool unload) {
            _listeners.Add(listener);
            unload.Add(new UnloadAction(() => RemoveListener(listener)));
        }

        public void AddListener(Action<T> listener) => _valueListeners.Add(listener);

        public void AddListener(Action<T> listener, UnloadPool unload) {
            _valueListeners.Add(listener);
            unload.Add(new UnloadAction(() => RemoveListener(listener)));
        }

        public void RemoveListener(Action listener) => _listeners.Remove(listener);

        public void RemoveListener(Action<T> listener) => _valueListeners.Remove(listener);

        public void Unload() {
            _listeners.Clear();
            _valueListeners.Clear();
        }

        public static implicit operator T(Observed<T> value) => value._value;
    }
}