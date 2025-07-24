// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
    public static class Observed {
        internal static int globalId;
        
        internal const int CAPACITY = 4;
        
        static Observed() => globalId = 0;
    }
    
#if ODIN_INSPECTOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class Observed<T> : IEquatable<Observed<T>>, IEquatable<T>, IUnload {
        public T value => _value;
        
    #if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup, HideLabel, OnValueChanged("@Set(_value)"), HideDuplicateReferenceBox, HideReferenceObjectPicker]
    #endif
        private T _value;
        
        private readonly int _id;
        private readonly List<ActionListener> _listeners;
        private readonly List<ActionListener<T>> _listenersValue;
        private readonly List<ActionListener<T, T>> _listenersChange;
        
        public Observed(T data, int capacity = Observed.CAPACITY) {
            _value = data;
            _id = Observed.globalId++;
            _listeners = new List<ActionListener>(capacity);
            _listenersValue = new List<ActionListener<T>>(capacity);
            _listenersChange = new List<ActionListener<T, T>>(capacity);
        }
        
        public Observed() {
            _value = default;
            _id = Observed.globalId++;
            _listeners = new List<ActionListener>(Observed.CAPACITY);
            _listenersValue = new List<ActionListener<T>>(Observed.CAPACITY);
            _listenersChange = new List<ActionListener<T, T>>(Observed.CAPACITY);
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
        public void AddListener(ActionListener listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener) => _listenersValue.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T, T> listener) => _listenersChange.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T, T> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersChange.Remove(listener)));
        }
        
    #endregion
        
    #region ByPriority
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListenerFirst(ActionListener listener) {
            if (_listeners.Count > 0) {
                _listeners.Insert(0, listener);
            } else {
                AddListener(listener);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListenerFirst(ActionListener listener, UnloadPool unload) {
            AddListenerFirst(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListenerFirst(ActionListener<T> listener) {
            if (_listenersValue.Count > 0) {
                _listenersValue.Insert(0, listener);
            } else {
                AddListener(listener);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListenerFirst(ActionListener<T> listener, UnloadPool unload) {
            AddListenerFirst(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListenerLast(ActionListener listener) {
            if (_listeners.Count > 0) {
                _listeners.Insert(_listeners.Count - 1, listener);
            } else {
                AddListener(listener);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListenerLast(ActionListener listener, UnloadPool unload) {
            AddListenerLast(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListenerLast(ActionListener<T> listener) {
            if (_listenersValue.Count > 0) {
                _listenersValue.Insert(_listenersValue.Count - 1, listener);
            } else {
                AddListener(listener);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListenerLast(ActionListener<T> listener, UnloadPool unload) {
            AddListenerLast(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T> listener) => _listenersValue.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T, T> listener) => _listenersChange.Remove(listener);
        
    #endregion
        
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
            _listenersChange.Clear();
        }
        
        public static implicit operator T(Observed<T> observed) => observed.value;
        
        public override string ToString() => $"{value}";
        
        public override int GetHashCode() => _id;
        
    #if UNITY_EDITOR && ODIN_INSPECTOR
        private static readonly bool _isInt = typeof(T) == typeof(int);
        private static readonly bool _isFloat = typeof(T) == typeof(float);
        
        [Button("x2"), HorizontalGroup, ShowIf("_isInt")]
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
        
        [Button("x2"), HorizontalGroup, ShowIf("_isFloat")]
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
        
        [Button("x0.5"), HorizontalGroup, ShowIf("_isInt")]
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
        
        [Button("x0.5"), HorizontalGroup, ShowIf("_isFloat")]
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