using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TinyMVC.Loop;
using TinyMVC.Modules.Networks.Extensions;
using TinyMVC.ReactiveFields;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.Modules.Networks.ReactiveFields {
    public sealed class NetListener<T> : IEquatable<NetListener<T>>, IEquatable<T>, IUnload {
        public T value => _value;
        
        [ShowInInspector, HorizontalGroup, HideLabel, HideDuplicateReferenceBox, HideReferenceObjectPicker]
        private T _value;
        
        public readonly ushort key;
        public readonly byte[] group;
        
        private readonly List<ActionListener<T>> _listenersValue;
        
        [Obsolete("Can`t use without parameters!", true)]
        public NetListener(ushort key) {
            // Do Nothing
        }
        
        [Obsolete("Can`t use without parameters!", true)]
        public NetListener(T value, ushort key) {
            // Do Nothing
        }
        
        public NetListener(ushort key, params byte[] group) : this(default, key, group) { }
        
        public NetListener(T value, ushort key, params byte[] group) {
            _value = value;
            
            this.key = key;
            this.group = group;
            
            _listenersValue = new List<ActionListener<T>>();
        }
        
        private void SetValue(object obj) {
            if (obj is T newValue) {
                _value = newValue;
                _listenersValue.Invoke(newValue);
            }
        }
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener) {
            if (_listenersValue.Count == 0) {
                NetService.AddRead(SetValue, key, group);
            }
            
            _listenersValue.Add(listener);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => RemoveListener(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T> listener) {
            _listenersValue.Remove(listener);
            
            if (_listenersValue.Count == 0) {
                NetService.RemoveRead(SetValue, key, group);
            }
        }
        
    #endregion
        
        public void Unload() {
            if (_listenersValue.Count > 0) {
                NetService.RemoveRead(SetValue, key, group);
            }
            
            _listenersValue.Clear();
        }
        
        public static implicit operator T(NetListener<T> observed) => observed.value;
        
        public override string ToString() => $"{value}";
        
        public override int GetHashCode() => HashCode.Combine(key, group);
        
        public bool Equals(NetListener<T> other) => other != null && other.key == key && other.group.EqualsValues(group);
        
        public bool Equals(T other) => other != null && other.Equals(value);
        
        public override bool Equals(object obj) => obj is NetListener<T> other && other.key == key && other.group.EqualsValues(group);
    }
}