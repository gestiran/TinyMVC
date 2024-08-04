﻿using System;
using System.Collections.Generic;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
    #if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
    #endif
    public sealed class ObservedDictionary<TKey, TValue> {
        public int count => _value.Count;
        
        private readonly List<Action> _onAdd;
        private readonly List<Action<TValue>> _onAddWithValue;
        private readonly List<Action> _onRemove;
        private readonly List<Action<TValue>> _onRemoveWithValue;
        
        #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, LabelText("Elements"), ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ListElementLabelName =
 "@GetType().Name")]
        private List<TValue> _inspectorDisplay;
        #endif
        
        private Dictionary<TKey, TValue> _value;
        
        private const int _CAPACITY = 16;
        
        public ObservedDictionary() : this(new Dictionary<TKey, TValue>()) { }
        
        public ObservedDictionary(int capacity) : this(new Dictionary<TKey, TValue>(capacity)) { }
        
        public ObservedDictionary(Dictionary<TKey, TValue> value) {
            _value = value;
            _onAdd = new List<Action>(_CAPACITY);
            _onAddWithValue = new List<Action<TValue>>(_CAPACITY);
            _onRemove = new List<Action>(_CAPACITY);
            _onRemoveWithValue = new List<Action<TValue>>(_CAPACITY);
            
            #if ODIN_INSPECTOR && UNITY_EDITOR
            _inspectorDisplay = new List<TValue>();

            foreach (TValue data in value.Values) {
                _inspectorDisplay.Add(data);
            }
            
            #endif
        }
        
        public void Add(TKey key, TValue value) {
            _value.Add(key, value);
            _onAdd.Invoke();
            _onAddWithValue.Invoke(value);
            
            #if ODIN_INSPECTOR && UNITY_EDITOR
            _inspectorDisplay.Add(value);
            #endif
        }
        
        public void RemoveByKey(TKey key) {
            if (_value.Remove(key, out TValue value) == false) {
                return;
            }
            
            _onRemove.Invoke();
            _onRemoveWithValue.Invoke(value);
            
            #if ODIN_INSPECTOR && UNITY_EDITOR
            _inspectorDisplay.Remove(value);
            #endif
        }
        
        public void RemoveRange(List<TValue> values) {
            KeyValuePair<TKey, TValue>[] dataPair = new KeyValuePair<TKey, TValue>[_value.Count];
            int dataId = 0;
            
            foreach (KeyValuePair<TKey, TValue> data in _value) {
                dataPair[dataId++] = data;
            }
            
            for (int valueId = 0; valueId < values.Count; valueId++) {
                TValue value = values[valueId];
                
                for (dataId = 0; dataId < dataPair.Length; dataId++) {
                    if (!dataPair[dataId].Value.Equals(value)) {
                        continue;
                    }
                    
                    _value.Remove(dataPair[dataId].Key);
                    _onRemove.Invoke();
                    _onRemoveWithValue.Invoke(value);
                    
                    #if ODIN_INSPECTOR && UNITY_EDITOR
                    _inspectorDisplay.Remove(value);
                    #endif
                    break;
                }
            }
        }
        
        public bool TryGetValue(TKey key, out TValue value) => _value.TryGetValue(key, out value);
        
        public bool ContainsKey(TKey key) => _value.ContainsKey(key);
        
        public IEnumerator<TKey> ForEachKeys() {
            TKey[] keys = new TKey[_value.Count];
            int keyId = 0;
            
            foreach (TKey key in _value.Keys) {
                keys[keyId++] = key;
            }
            
            for (keyId = 0; keyId < keys.Length; keyId++) {
                yield return keys[keyId];
            }
        }
        
        public IEnumerable<TValue> ForEachValues() {
            TValue[] values = new TValue[_value.Count];
            int valueId = 0;
            
            foreach (TValue value in _value.Values) {
                values[valueId++] = value;
            }
            
            for (valueId = 0; valueId < values.Length; valueId++) {
                yield return values[valueId];
            }
        }
        
        public void AddOnAddListener(Action listener) => _onAdd.Add(listener);
        
        public void AddOnAddListener(Action listener, UnloadPool unload) {
            _onAdd.Add(listener);
            unload.Add(new UnloadAction(() => _onAdd.Remove(listener)));
        }
        
        public void AddOnAddListener(Action<TValue> listener) => _onAddWithValue.Add(listener);
        
        public void AddOnAddListener(Action<TValue> listener, UnloadPool unload) {
            _onAddWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onAddWithValue.Remove(listener)));
        }
        
        public void RemoveOnAddListener(Action listener) => _onAdd.Remove(listener);
        
        public void RemoveOnAddListener(Action<TValue> listener) => _onAddWithValue.Remove(listener);
        
        public void AddOnRemoveListener(Action listener) => _onRemove.Add(listener);
        
        public void AddOnRemoveListener(Action listener, UnloadPool unload) {
            _onRemove.Add(listener);
            unload.Add(new UnloadAction(() => _onRemove.Remove(listener)));
        }
        
        public void AddOnRemoveListener(Action<TValue> listener) => _onRemoveWithValue.Add(listener);
        
        public void AddOnRemoveListener(Action<TValue> listener, UnloadPool unload) {
            _onRemoveWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onRemoveWithValue.Remove(listener)));
        }
        
        public void RemoveOnRemoveListener(Action listener) => _onRemove.Remove(listener);
        
        public void RemoveOnRemoveListener(Action<TValue> listener) => _onRemoveWithValue.Remove(listener);
    }
}