using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.ReactiveFields {
    public static class Observed {
        internal static int globalId;
        
        static Observed() => globalId = 0;
    }
    
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
    public sealed class Observed<T> : IEquatable<Observed<T>>, IEquatable<T>, IUnload {
        public T value => _value;
        
        [ShowInInspector, HorizontalGroup, HideLabel, OnValueChanged("@Set(_value)"), HideDuplicateReferenceBox, HideReferenceObjectPicker]
        private T _value;
        
        private readonly int _id;
        private readonly List<ActionListener> _listeners;
        private readonly List<ActionListener<T>> _listenersValue;
        
        public Observed(T data) : this() => _value = data;
        
        public Observed() {
            _id = Observed.globalId++;
            _listeners = new List<ActionListener>(16);
            _listenersValue = new List<ActionListener<T>>(16);
        }
        
        public void SetSilent(T newValue) => _value = newValue;
        
        public void Set(T newValue) {
            _value = newValue;
            _listeners.Invoke();
            _listenersValue.Invoke(newValue);
        }
        
    #region Add
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener) => _listenersValue.Add(listener);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
    #endregion
        
    #region ByPriority
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListenerFirst(ActionListener listener) {
            if (_listeners.Count > 0) {
                _listeners.Insert(0, listener);
            } else {
                AddListener(listener);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListenerFirst(ActionListener listener, UnloadPool unload) {
            AddListenerFirst(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListenerFirst(ActionListener<T> listener) {
            if (_listenersValue.Count > 0) {
                _listenersValue.Insert(0, listener);
            } else {
                AddListener(listener);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListenerFirst(ActionListener<T> listener, UnloadPool unload) {
            AddListenerFirst(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListenerLast(ActionListener listener) {
            if (_listeners.Count > 0) {
                _listeners.Insert(_listeners.Count - 1, listener);
            } else {
                AddListener(listener);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListenerLast(ActionListener listener, UnloadPool unload) {
            AddListenerLast(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListenerLast(ActionListener<T> listener) {
            if (_listenersValue.Count > 0) {
                _listenersValue.Insert(_listenersValue.Count - 1, listener);
            } else {
                AddListener(listener);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void AddListenerLast(ActionListener<T> listener, UnloadPool unload) {
            AddListenerLast(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T> listener) => _listenersValue.Remove(listener);
        
    #endregion
        
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
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