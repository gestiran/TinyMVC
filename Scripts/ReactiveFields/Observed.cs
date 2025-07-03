using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.ReactiveFields {
    public static class Observed {
        internal static int globalId;
        
        internal const int CAPACITY = 4;
        
        static Observed() => globalId = 0;
    }
    
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
    public sealed class Observed<T> : IEquatable<Observed<T>>, IEquatable<T>, IUnload {
        public T value => _value;
        
        [ShowInInspector, HorizontalGroup, HideLabel, OnValueChanged("@Set(_value)"), HideDuplicateReferenceBox, HideReferenceObjectPicker]
        private T _value;
        
        private readonly int _id;
        private readonly Dictionary<int, ActionListener> _listeners;
        private readonly Dictionary<int, ActionListener<T>> _listenersValue;
        private readonly Dictionary<int, ActionListener<T, T>> _listenersChange;
        
        public Observed(T data, int capacity = Observed.CAPACITY) {
            _value = data;
            _id = Observed.globalId++;
            _listeners = new Dictionary<int, ActionListener>(capacity);
            _listenersValue = new Dictionary<int, ActionListener<T>>(capacity);
            _listenersChange = new Dictionary<int, ActionListener<T, T>>(capacity);
        }
        
        public Observed() {
            _value = default;
            _id = Observed.globalId++;
            _listeners = new Dictionary<int, ActionListener>(Observed.CAPACITY);
            _listenersValue = new Dictionary<int, ActionListener<T>>(Observed.CAPACITY);
            _listenersChange = new Dictionary<int, ActionListener<T, T>>(Observed.CAPACITY);
        }
        
        public void SetSilent(T newValue) => _value = newValue;
        
        public void Set(T newValue) {
            T current = _value;
            _value = newValue;
            _listeners.Invoke();
            _listenersValue.Invoke(newValue);
            _listenersChange.Invoke(current, newValue);
        }
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener.GetHashCode(), listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener.GetHashCode())));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener) => _listenersValue.Add(listener.GetHashCode(), listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener.GetHashCode())));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddChangeListener(ActionListener<T, T> listener) => _listenersChange.Add(listener.GetHashCode(), listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddChangeListener(ActionListener<T, T> listener, UnloadPool unload) {
            AddChangeListener(listener);
            unload.Add(new UnloadAction(() => _listenersChange.Remove(listener.GetHashCode())));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener.GetHashCode());
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T> listener) => _listenersValue.Remove(listener.GetHashCode());
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveChangeListener(ActionListener<T, T> listener) => _listenersChange.Remove(listener.GetHashCode());
        
    #endregion
        
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
            _listenersChange.Clear();
        }
        
        public static implicit operator T(Observed<T> observed) => observed.value;
        
        public override string ToString() => $"{value}";
        
        public override int GetHashCode() => _id;
        
    #if UNITY_EDITOR
        private bool IsInt() => typeof(T) == typeof(int);
        
        private bool IsFloat() => typeof(T) == typeof(float);
        
        [Button("x2"), HorizontalGroup, ShowIf("IsInt")]
        private void AddInt() {
            if (_value is int intValue) {
                if (intValue == 0) {
                    intValue = 10;
                } else {
                    intValue *= 2;
                }
                
                if (intValue is T result) {
                    Set(result);
                }
            }
        }
        
        [Button("x2"), HorizontalGroup, ShowIf("IsFloat")]
        private void AddFloat() {
            if (_value is float floatValue) {
                if (floatValue == 0f) {
                    floatValue = 10f;
                } else {
                    floatValue *= 2f;
                }
                
                if (floatValue is T result) {
                    Set(result);
                }
            }
        }
        
        [Button("x0.5"), HorizontalGroup, ShowIf("IsInt")]
        private void SubtractInt() {
            if (_value is int intValue) {
                if (intValue > 0 && intValue <= 10) {
                    intValue = 0;
                } else {
                    intValue /= 2;
                }
                
                if (intValue is T result) {
                    Set(result);
                }
            }
        }
        
        [Button("x0.5"), HorizontalGroup, ShowIf("IsFloat")]
        private void SubtractFloat() {
            if (_value is float floatValue) {
                if (floatValue > 0f && floatValue <= 10f) {
                    floatValue = 0f;
                } else {
                    floatValue *= 0.5f;
                }
                
                if (floatValue is T result) {
                    Set(result);
                }
            }
        }
        
    #endif
        public bool Equals(Observed<T> other) => other != null && other._id == _id;
        
        public bool Equals(T other) => other != null && other.Equals(value);
        
        public override bool Equals(object obj) => obj is Observed<T> other && other._id == _id;
    }
}