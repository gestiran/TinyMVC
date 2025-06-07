using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.Modules.Networks.ReactiveFields {
    public sealed class NetPartListener<T> : IEquatable<NetPartListener<T>>, IEquatable<T>, IUnload {
        public T value => _value;
        
        [ShowInInspector, HorizontalGroup, HideLabel, HideDuplicateReferenceBox, HideReferenceObjectPicker]
        private T _value;
        
        public readonly ushort group;
        public readonly byte key;
        
        private readonly List<ActionListener<ushort, T>> _listenersValue;
        
        public NetPartListener(ushort group, byte key, T value = default) {
            this.group = group;
            this.key = key;
            
            _value = value;
            
            _listenersValue = new List<ActionListener<ushort, T>>();
        }
        
        private void SetValue(ushort partValue, object obj) {
            if (obj == null) {
                _value = default;
                _listenersValue.Invoke(partValue, _value);
            } else if (obj is T newValue) {
                _value = newValue;
                _listenersValue.Invoke(partValue, newValue);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<ushort, T> listener) {
            if (_listenersValue.Count == 0) {
                NetSyncService.AddRead(group, 0, key, SetValue);
            }
            
            _listenersValue.Add(listener);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<ushort, T> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => RemoveListener(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<ushort, T> listener) {
            _listenersValue.Remove(listener);
            
            if (_listenersValue.Count == 0) {
                NetSyncService.RemoveRead(group, 0, key, SetValue);
            }
        }
        
        public void Unload() {
            if (_listenersValue.Count > 0) {
                NetSyncService.RemoveRead(group, 0, key, SetValue);
            }
            
            _listenersValue.Clear();
        }
        
        public static implicit operator T(NetPartListener<T> observed) => observed.value;
        
        public override string ToString() => $"{value}";
        
        public override int GetHashCode() => HashCode.Combine(group, key);
        
        public bool Equals(NetPartListener<T> other) => other != null && other.group == group && other.key == key;
        
        public bool Equals(T other) => other != null && other.Equals(value);
        
        public override bool Equals(object obj) => obj is NetPartListener<T> other && other.group == group && other.key == key;
    }
}