// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;
using TinyUtilities;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class ObservedList<T> : IEnumerable<T>, IEnumerator<T> {
        public int count => _value.Count;
        public T Current => _value[_currentId];
        object IEnumerator.Current => _value[_currentId];
        
        private readonly List<ActionListener> _onAdd;
        private readonly List<ActionListener<T>> _onAddWithValue;
        private readonly List<ActionListener> _onRemove;
        private readonly List<ActionListener<T>> _onRemoveWithValue;
        private readonly List<ActionListener> _onClear;
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, HideReferenceObjectPicker, HideDuplicateReferenceBox,
         ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, DefaultExpandedState = false)]
    #endif
        private List<T> _value;
        
        private int _currentId;
        private bool _lock;
        
        private const int _ASYNC_ANR_MS = 64;
        
        public ObservedList(int capacity = Observed.CAPACITY) : this(new List<T>(), capacity) { }
        
        public ObservedList(T[] value, int capacity = Observed.CAPACITY) : this(value.ToList(), capacity) { }
        
        public ObservedList(List<T> value, int capacity = Observed.CAPACITY) {
            _value = value;
            _onAdd = new List<ActionListener>(capacity);
            _onAddWithValue = new List<ActionListener<T>>(capacity);
            _onRemove = new List<ActionListener>(capacity);
            _onRemoveWithValue = new List<ActionListener<T>>(capacity);
            _onClear = new List<ActionListener>(capacity);
            _currentId = -1;
        }
        
        public T this[int index] {
            get => _value[index];
            set {
                _onRemove.Invoke();
                _onRemoveWithValue.Invoke(_value[index]);
                
                _value[index] = value;
                
                _onAdd.Invoke();
                _onAddWithValue.Invoke(value);
            }
        }
        
        [Obsolete("Can't add nothing!", true)]
        public void Add() {
            // Do nothing
        }
        
        public void Add([NotNull] params T[] values) {
            _value.AddRange(values);
            _onAdd.Invoke();
            _onAddWithValue.Invoke(values);
        }
        
        public void Add([NotNull] T value) {
            _value.Add(value);
            _onAdd.Invoke();
            _onAddWithValue.Invoke(value);
        }
        
        [Obsolete("Can't add nothing!", true)]
        public UniTask AddAsync() => default;
        
        public UniTask AddAsync([NotNull] params T[] values) => AddAsync(_ASYNC_ANR_MS, AsyncUtility.token, values);
        
        [Obsolete("Can't add nothing!", true)]
        public UniTask AddAsync(CancellationToken cancellation) => default;
        
        public UniTask AddAsync(CancellationToken cancellation, [NotNull] params T[] values) => AddAsync(_ASYNC_ANR_MS, cancellation, values);
        
        [Obsolete("Can't add nothing!", true)]
        public UniTask AddAsync(int anr, CancellationToken cancellation) => default;
        
        public async UniTask AddAsync(int anr, CancellationToken cancellation, [NotNull] params T[] values) {
            if (_lock) {
            #if UNITY_EDITOR || PERFORMANCE_DEBUG
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }
            
            _lock = true;
            _value.AddRange(values);
            DateTime now = DateTime.Now;
            
            for (int i = _onAdd.Count - 1; i >= 0; i--) {
                _onAdd[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            for (int i = _onAddWithValue.Count - 1; i >= 0; i--) {
                for (int j = 0; j < values.Length; j++) {
                    _onAddWithValue[i].Invoke(values[j]);
                }
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            _lock = false;
        }
        
        public UniTask AddAsync([NotNull] T value) => AddAsync(_ASYNC_ANR_MS, AsyncUtility.token, value);
        
        public UniTask AddAsync(CancellationToken cancellation, [NotNull] T value) => AddAsync(_ASYNC_ANR_MS, cancellation, value);
        
        public async UniTask AddAsync(int anr, CancellationToken cancellation, [NotNull] T value) {
            if (_lock) {
            #if UNITY_EDITOR || PERFORMANCE_DEBUG
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }
            
            _lock = true;
            _value.Add(value);
            DateTime now = DateTime.Now;
            
            for (int i = _onAdd.Count - 1; i >= 0; i--) {
                _onAdd[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            for (int i = _onAddWithValue.Count - 1; i >= 0; i--) {
                _onAddWithValue[i].Invoke(value);
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            _lock = false;
        }
        
        [Obsolete("Can't remove nothing!", true)]
        public void Remove() {
            // Do nothing
        }
        
        public void Remove([NotNull] params T[] values) {
            for (int i = values.Length - 1; i >= 0; i--) {
                _value.Remove(values[i]);
            }
            
            _onRemove.Invoke();
            _onRemoveWithValue.Invoke(values);
        }
        
        public bool Remove([NotNull] T value) {
            if (_value.Remove(value)) {
                _onRemove.Invoke();
                _onRemoveWithValue.Invoke(value);
                return true;
            }
            
            return false;
        }
        
        public void RemoveAll() {
            for (int i = count - 1; i >= 0; i--) {
                T value = _value[i];
                _value.RemoveAt(i);
                _onRemove.Invoke();
                _onRemoveWithValue.Invoke(value);
            }
        }
        
        public UniTask RemoveAsync([NotNull] params T[] values) => RemoveAsync(_ASYNC_ANR_MS, AsyncUtility.token, values);
        
        public UniTask RemoveAsync(CancellationToken cancellation, [NotNull] params T[] values) => RemoveAsync(_ASYNC_ANR_MS, cancellation, values);
        
        public async UniTask RemoveAsync(int anr, CancellationToken cancellation, [NotNull] params T[] values) {
            if (_lock) {
            #if UNITY_EDITOR || PERFORMANCE_DEBUG
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }
            
            _lock = true;
            
            for (int i = values.Length - 1; i >= 0; i--) {
                _value.Remove(values[i]);
            }
            
            DateTime now = DateTime.Now;
            
            for (int i = _onRemove.Count - 1; i >= 0; i--) {
                _onRemove[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            for (int i = _onRemoveWithValue.Count - 1; i >= 0; i--) {
                for (int j = 0; j < values.Length; j++) {
                    _onRemoveWithValue[i].Invoke(values[j]);
                }
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            _lock = false;
        }
        
        public UniTask RemoveAsync([NotNull] T value) => RemoveAsync(_ASYNC_ANR_MS, AsyncUtility.token, value);
        
        public UniTask RemoveAsync(CancellationToken cancellation, [NotNull] T value) => RemoveAsync(_ASYNC_ANR_MS, cancellation, value);
        
        public async UniTask RemoveAsync(int anr, CancellationToken cancellation, [NotNull] T value) {
            if (_lock) {
            #if UNITY_EDITOR || PERFORMANCE_DEBUG
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }
            
            _lock = true;
            _value.Remove(value);
            DateTime now = DateTime.Now;
            
            for (int i = _onRemove.Count - 1; i >= 0; i--) {
                _onRemove[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            for (int i = _onRemoveWithValue.Count - 1; i >= 0; i--) {
                _onRemoveWithValue[i].Invoke(value);
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            _lock = false;
        }
        
        public void Clear() {
            _value.Clear();
            _onClear.Invoke();
        }
        
        public int IndexOf(T element) => _value.IndexOf(element);
        
        public bool Contains(T element) => _value.Contains(element);
        
        public void RemoveAt(int id) {
            T element = _value[id];
            _value.RemoveAt(id);
            _onRemove.Invoke();
            _onRemoveWithValue.Invoke(element);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener listener) => _onAdd.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener listener, UnloadPool unload) {
            _onAdd.Add(listener);
            unload.Add(new UnloadAction(() => _onAdd.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener<T> listener) => _onAddWithValue.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener<T> listener, UnloadPool unload) {
            _onAddWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onAddWithValue.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnAddListener(ActionListener listener) => _onAdd.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnAddListener(ActionListener<T> listener) => _onAddWithValue.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener listener) => _onRemove.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener listener, UnloadPool unload) {
            _onRemove.Add(listener);
            unload.Add(new UnloadAction(() => _onRemove.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener<T> listener) => _onRemoveWithValue.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener<T> listener, UnloadPool unload) {
            _onRemoveWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onRemoveWithValue.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnRemoveListener(ActionListener listener) => _onRemove.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnRemoveListener(ActionListener<T> listener) => _onRemoveWithValue.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnClearListener(ActionListener listener) => _onClear.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnClearListener(ActionListener listener, UnloadPool unload) {
            _onClear.Add(listener);
            unload.Add(new UnloadAction(() => _onClear.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnClearListener(ActionListener listener) => _onClear.Remove(listener);
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public IEnumerator<T> GetEnumerator() {
            foreach (T value in _value) {
                yield return value;
            }
        }
        
        public bool MoveNext() {
            _currentId++;
            
            return _currentId < _value.Count;
        }
        
        public void Reset() => _currentId = -1;
        
        public void Dispose() {
            Reset();
            _value = null;
            _onAdd.Clear();
            _onAddWithValue.Clear();
            _onRemove.Clear();
            _onRemoveWithValue.Clear();
            _onClear.Clear();
        }
    }
}