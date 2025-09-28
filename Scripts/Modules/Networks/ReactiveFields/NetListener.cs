// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyReactive;
using TinyReactive.Extensions;
using TinyReactive.Fields;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Modules.Networks.ReactiveFields {
    public sealed class NetListener<T> : IEquatable<NetListener<T>>, IEquatable<T>, IUnload {
        public T value => _value;
        
    #if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup, HideLabel, HideDuplicateReferenceBox, HideReferenceObjectPicker]
    #endif
        private T _value;
        
        public readonly ushort group;
        public readonly ushort part;
        public readonly byte key;
        
        private readonly List<ActionListener<T>> _listenersValue;
        
        public NetListener(ushort group, ushort part, byte key, T value = default) {
            this.group = group;
            this.part = part;
            this.key = key;
            
            _value = value;
            
            _listenersValue = new List<ActionListener<T>>();
        }
        
        private void SetValue(ushort _, object obj) {
            if (obj == null) {
                _value = default;
                _listenersValue.Invoke(_value);
            } else if (obj is T newValue) {
                _value = newValue;
                _listenersValue.Invoke(newValue);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener) {
            if (_listenersValue.Count == 0) {
                NetSyncService.AddRead(group, part, key, SetValue);
            }
            
            _listenersValue.Add(listener);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => RemoveListener(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T> listener) {
            _listenersValue.Remove(listener);
            
            if (_listenersValue.Count == 0) {
                NetSyncService.RemoveRead(group, part, key, SetValue);
            }
        }
        
        public void Unload() {
            if (_listenersValue.Count > 0) {
                NetSyncService.RemoveRead(group, part, key, SetValue);
            }
            
            _listenersValue.Clear();
        }
        
        public static implicit operator T(NetListener<T> observed) => observed.value;
        
        public override string ToString() => $"{value}";
        
        public override int GetHashCode() => HashCode.Combine(group, part, key);
        
        public bool Equals(NetListener<T> other) => other != null && other.group == group && other.part == part && other.key == key;
        
        public bool Equals(T other) => other != null && other.Equals(value);
        
        public override bool Equals(object obj) => obj is NetListener<T> other && other.group == group && other.part == part && other.key == key;
    }
}