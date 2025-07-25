﻿// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class ObservedDictionary<TKey, TValue> {
        public int count => root.Count;
        
        private readonly List<ActionListener> _onAdd;
        private readonly List<ActionListener<TValue>> _onAddWithValue;
        private readonly List<ActionListener> _onRemove;
        private readonly List<ActionListener<TValue>> _onRemoveWithValue;
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, LabelText("Elements")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ListElementLabelName = "@GetType().Name")]
        private List<TValue> _inspectorDisplay;
    #endif
        
        internal readonly Dictionary<TKey, TValue> root;
        
        public ObservedDictionary(int capacity = Observed.CAPACITY) : this(new Dictionary<TKey, TValue>(capacity)) { }
        
        public ObservedDictionary(Dictionary<TKey, TValue> value, int capacity = Observed.CAPACITY) {
            root = value;
            _onAdd = new List<ActionListener>(capacity);
            _onAddWithValue = new List<ActionListener<TValue>>(capacity);
            _onRemove = new List<ActionListener>(capacity);
            _onRemoveWithValue = new List<ActionListener<TValue>>(capacity);
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            _inspectorDisplay = new List<TValue>();
            
            foreach (TValue data in value.Values) {
                _inspectorDisplay.Add(data);
            }
            
        #endif
        }
        
        public void Add(TKey key, TValue value) {
            root.Add(key, value);
            _onAdd.Invoke();
            _onAddWithValue.Invoke(value);
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            _inspectorDisplay.Add(value);
        #endif
        }
        
        public bool Remove(TKey key) {
            if (root.Remove(key, out TValue value) == false) {
                return false;
            }
            
            _onRemove.Invoke();
            _onRemoveWithValue.Invoke(value);
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            _inspectorDisplay.Remove(value);
        #endif
            
            return true;
        }
        
        public void RemoveRange(List<TValue> values) {
            KeyValuePair<TKey, TValue>[] dataPair = new KeyValuePair<TKey, TValue>[root.Count];
            int dataId = 0;
            
            foreach (KeyValuePair<TKey, TValue> data in root) {
                dataPair[dataId++] = data;
            }
            
            for (int valueId = 0; valueId < values.Count; valueId++) {
                TValue value = values[valueId];
                
                for (dataId = 0; dataId < dataPair.Length; dataId++) {
                    if (!dataPair[dataId].Value.Equals(value)) {
                        continue;
                    }
                    
                    root.Remove(dataPair[dataId].Key);
                    _onRemove.Invoke();
                    _onRemoveWithValue.Invoke(value);
                    
                #if ODIN_INSPECTOR && UNITY_EDITOR
                    _inspectorDisplay.Remove(value);
                #endif
                    break;
                }
            }
        }
        
        public bool TryGetValue(TKey key, out TValue value) => root.TryGetValue(key, out value);
        
        public bool ContainsKey(TKey key) => root.ContainsKey(key);
        
        public IEnumerator<TKey> ForEachKeys() {
            TKey[] keys = new TKey[root.Count];
            int keyId = 0;
            
            foreach (TKey key in root.Keys) {
                keys[keyId++] = key;
            }
            
            foreach (TKey key in keys) {
                yield return key;
            }
        }
        
        public IEnumerable<TValue> ForEachValues() {
            TValue[] values = new TValue[root.Count];
            int valueId = 0;
            
            foreach (TValue value in root.Values) {
                values[valueId++] = value;
            }
            
            foreach (TValue value in values) {
                yield return value;
            }
        }
        
        public void AddOnAddListener(ActionListener listener) => _onAdd.Add(listener);
        
        public void AddOnAddListener(ActionListener listener, UnloadPool unload) {
            _onAdd.Add(listener);
            unload.Add(new UnloadAction(() => _onAdd.Remove(listener)));
        }
        
        public void AddOnAddListener(ActionListener<TValue> listener) => _onAddWithValue.Add(listener);
        
        public void AddOnAddListener(ActionListener<TValue> listener, UnloadPool unload) {
            _onAddWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onAddWithValue.Remove(listener)));
        }
        
        public void RemoveOnAddListener(ActionListener listener) => _onAdd.Remove(listener);
        
        public void RemoveOnAddListener(ActionListener<TValue> listener) => _onAddWithValue.Remove(listener);
        
        public void AddOnRemoveListener(ActionListener listener) => _onRemove.Add(listener);
        
        public void AddOnRemoveListener(ActionListener listener, UnloadPool unload) {
            _onRemove.Add(listener);
            unload.Add(new UnloadAction(() => _onRemove.Remove(listener)));
        }
        
        public void AddOnRemoveListener(ActionListener<TValue> listener) => _onRemoveWithValue.Add(listener);
        
        public void AddOnRemoveListener(ActionListener<TValue> listener, UnloadPool unload) {
            _onRemoveWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onRemoveWithValue.Remove(listener)));
        }
        
        public void RemoveOnRemoveListener(ActionListener listener) => _onRemove.Remove(listener);
        
        public void RemoveOnRemoveListener(ActionListener<TValue> listener) => _onRemoveWithValue.Remove(listener);
    }
}