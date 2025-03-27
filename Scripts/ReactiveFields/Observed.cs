using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.ReactiveFields {
    public static class Observed {
        internal static int globalId;
        
        static Observed() => globalId = 0;
    }
    
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
    public sealed class Observed<T> : IEquatable<Observed<T>>, IEquatable<T> {
        public T value => _value;
        
        [ShowInInspector, HorizontalGroup, HideLabel, OnValueChanged("@Set(_value)"), HideDuplicateReferenceBox, HideReferenceObjectPicker]
        private T _value;
        
        internal readonly int id;
        
        public Observed(T data) : this() => _value = data;
        
        public Observed() => id = Observed.globalId++;
        
        public void SetSilent(T newValue) => _value = newValue;
        
        public void Set(T newValue) {
            _value = newValue;
            
            if (Listeners.pool.TryGetValue(id, out List<ActionListener> listeners)) {
                listeners.Invoke();
            }
            
            if (Listeners<T>.pool.TryGetValue(id, out List<ActionListener<T>> valueListeners)) {
                valueListeners.Invoke(newValue);
            }
        }
        
        public static implicit operator T(Observed<T> observed) => observed.value;
        
        public override string ToString() => $"{value}";
        
        public override int GetHashCode() => id;
        
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
        public bool Equals(Observed<T> other) => other != null && other.id == id;
        
        public bool Equals(T other) => other != null && other.Equals(value);
        
        public override bool Equals(object obj) => obj is Observed<T> other && other.id == id;
    }
}