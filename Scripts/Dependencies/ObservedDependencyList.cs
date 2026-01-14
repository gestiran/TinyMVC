// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TinyReactive;
using TinyReactive.Fields;
using TinyUtilities;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [HideLabel, ShowInInspector, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class ObservedDependencyList<T> : IEnumerable<T>, IEnumerator<T>, IDependency where T : IDependency {
        public int count => _list.Count;
        public T Current => _list[_currentId];
        object IEnumerator.Current => _list[_currentId];
        
        private readonly LazyList<ActionListener> _onAdd;
        private readonly LazyList<ActionListener<T>> _onAddWithValue;
        private readonly LazyList<ActionListener> _onRemove;
        private readonly LazyList<ActionListener<T>> _onRemoveWithValue;
        private readonly LazyList<ActionListener> _onClear;
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ListElementLabelName = "@ToString()")]
        [ShowInInspector, HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox, Searchable]
    #endif
        private List<T> _list;
        
        private int _currentId;
        private bool _lock;
        
        private const int _ASYNC_ANR_MS = 64;
        
        public ObservedDependencyList(int capacity = Observed.CAPACITY) : this(new List<T>(), capacity) { }
        
        public ObservedDependencyList(T[] value, int capacity = Observed.CAPACITY) : this(value.ToList(), capacity) { }
        
        public ObservedDependencyList([NotNull] DependencyPool<T> pool, int capacity = Observed.CAPACITY) {
            _list = new List<T>(pool.length);
            
            for (int valueId = 0; valueId < pool.length; valueId++) {
                _list.Add(pool[valueId]);
            }
            
            _onAdd = new LazyList<ActionListener>(capacity);
            _onAddWithValue = new LazyList<ActionListener<T>>(capacity);
            _onRemove = new LazyList<ActionListener>(capacity);
            _onRemoveWithValue = new LazyList<ActionListener<T>>(capacity);
            _onClear = new LazyList<ActionListener>(capacity);
            _currentId = -1;
        }
        
        public ObservedDependencyList([NotNull] params DependencyPool<T>[] pools) {
            int length = 0;
            
            for (int poolId = 0; poolId < pools.Length; poolId++) {
                length += pools[poolId].length;
            }
            
            _list = new List<T>(length);
            
            for (int poolId = 0; poolId < pools.Length; poolId++) {
                _list.AddRange(pools[poolId]);
            }
            
            _onAdd = new LazyList<ActionListener>(Observed.CAPACITY);
            _onAddWithValue = new LazyList<ActionListener<T>>(Observed.CAPACITY);
            _onRemove = new LazyList<ActionListener>(Observed.CAPACITY);
            _onRemoveWithValue = new LazyList<ActionListener<T>>(Observed.CAPACITY);
            _onClear = new LazyList<ActionListener>(Observed.CAPACITY);
            _currentId = -1;
        }
        
        public ObservedDependencyList(List<T> value, int capacity = Observed.CAPACITY) {
            _list = value;
            _onAdd = new LazyList<ActionListener>(capacity);
            _onAddWithValue = new LazyList<ActionListener<T>>(capacity);
            _onRemove = new LazyList<ActionListener>(capacity);
            _onRemoveWithValue = new LazyList<ActionListener<T>>(capacity);
            _onClear = new LazyList<ActionListener>(capacity);
            _currentId = -1;
        }
        
        public T this[int index] {
            get => _list[index];
            set {
                if (_onRemove.isDirty) {
                    _onRemove.Apply();
                }
                
                if (_onRemoveWithValue.isDirty) {
                    _onRemoveWithValue.Apply();
                }
                
                for (int i = 0; i < _onRemove.count; i++) {
                    _onRemove[i].Invoke();
                }
                
                for (int i = 0; i < _onRemoveWithValue.count; i++) {
                    _onRemoveWithValue[i].Invoke(_list[index]);
                }
                
                _list[index] = value;
                
                if (_onAdd.isDirty) {
                    _onAdd.Apply();
                }
                
                if (_onAddWithValue.isDirty) {
                    _onAddWithValue.Apply();
                }
                
                for (int i = 0; i < _onAdd.count; i++) {
                    _onAdd[i].Invoke();
                }
                
                for (int i = 0; i < _onAddWithValue.count; i++) {
                    _onAddWithValue[i].Invoke(value);
                }
            }
        }
        
        [Obsolete("Can`t use without parameters!", true)]
        public void Add() {
            // Do nothing
        }
        
        public void Add([NotNull] params T[] values) {
            _list.AddRange(values);
            
            if (_onAdd.isDirty) {
                _onAdd.Apply();
            }
            
            if (_onAddWithValue.isDirty) {
                _onAddWithValue.Apply();
            }
            
            for (int i = 0; i < _onAdd.count; i++) {
                _onAdd[i].Invoke();
            }
            
            for (int valueId = 0; valueId < values.Length; valueId++) {
                T value = values[valueId];
                
                for (int i = 0; i < _onAddWithValue.count; i++) {
                    _onAddWithValue[i].Invoke(value);
                }
            }
        }
        
        public void Add([NotNull] T value) {
            _list.Add(value);
            
            if (_onAdd.isDirty) {
                _onAdd.Apply();
            }
            
            if (_onAddWithValue.isDirty) {
                _onAddWithValue.Apply();
            }
            
            for (int i = 0; i < _onAdd.count; i++) {
                _onAdd[i].Invoke();
            }
            
            for (int i = 0; i < _onAddWithValue.count; i++) {
                _onAddWithValue[i].Invoke(value);
            }
        }
        
        [Obsolete("Can`t use without parameters!", true)]
        public UniTask AddAsync() => default;
        
        [Obsolete("Can`t use without parameters!", true)]
        public UniTask AddAsync(CancellationToken cancellation) => default;
        
        [Obsolete("Can`t use without parameters!", true)]
        public UniTask AddAsync(int anr, CancellationToken cancellation) => default;
        
        public UniTask AddAsync([NotNull] params T[] values) => AddAsync(_ASYNC_ANR_MS, AsyncUtility.token, values);
        
        public UniTask AddAsync(CancellationToken cancellation, [NotNull] params T[] values) => AddAsync(_ASYNC_ANR_MS, cancellation, values);
        
        public async UniTask AddAsync(int anr, CancellationToken cancellation, [NotNull] params T[] values) {
            if (_lock) {
            #if UNITY_EDITOR || PERFORMANCE_DEBUG
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }
            
            _lock = true;
            _list.AddRange(values);
            DateTime now = DateTime.Now;
            
            for (int i = _onAdd.count - 1; i >= 0; i--) {
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
            
            for (int i = _onAddWithValue.count - 1; i >= 0; i--) {
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
            _list.Add(value);
            DateTime now = DateTime.Now;
            
            for (int i = _onAdd.count - 1; i >= 0; i--) {
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
            
            for (int i = _onAddWithValue.count - 1; i >= 0; i--) {
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
        
        [Obsolete("Can`t use without parameters!", true)]
        public void Remove() {
            // Do nothing
        }
        
        public void Remove([NotNull] params T[] values) {
            if (_onRemove.isDirty) {
                _onRemove.Apply();
            }
            
            if (_onRemoveWithValue.isDirty) {
                _onRemoveWithValue.Apply();
            }
            
            foreach (T value in values) {
                int index = _list.IndexOf(value);
                
                if (index >= 0) {
                    for (int i = 0; i < _onRemove.count; i++) {
                        _onRemove[i].Invoke();
                    }
                    
                    for (int i = 0; i < _onRemoveWithValue.count; i++) {
                        _onRemoveWithValue[i].Invoke(value);
                    }
                    
                    _list.RemoveAt(index);
                }
            }
        }
        
        public bool Remove([NotNull] T value) {
            int index = _list.IndexOf(value);
            
            if (index >= 0) {
                if (_onRemove.isDirty) {
                    _onRemove.Apply();
                }
                
                if (_onRemoveWithValue.isDirty) {
                    _onRemoveWithValue.Apply();
                }
                
                for (int i = 0; i < _onRemove.count; i++) {
                    _onRemove[i].Invoke();
                }
                
                for (int i = 0; i < _onRemoveWithValue.count; i++) {
                    _onRemoveWithValue[i].Invoke(value);
                }
                
                _list.RemoveAt(index);
                
                return true;
            }
            
            return false;
        }
        
        [Obsolete("Can`t use without parameters!", true)]
        public UniTask RemoveAsync() => default; // Do nothing
        
        [Obsolete("Can`t use without parameters!", true)]
        public UniTask RemoveAsync(CancellationToken cancellation) => default; // Do nothing
        
        [Obsolete("Can`t use without parameters!", true)]
        public UniTask RemoveAsync(int anr, CancellationToken cancellation) => default; // Do nothing
        
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
                _list.Remove(values[i]);
            }
            
            DateTime now = DateTime.Now;
            
            for (int i = _onRemove.count - 1; i >= 0; i--) {
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
            
            for (int i = _onRemoveWithValue.count - 1; i >= 0; i--) {
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
            _list.Remove(value);
            DateTime now = DateTime.Now;
            
            for (int i = _onRemove.count - 1; i >= 0; i--) {
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
            
            for (int i = _onRemoveWithValue.count - 1; i >= 0; i--) {
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
            if (_onClear.isDirty) {
                _onClear.Apply();
            }
            
            for (int i = 0; i < _onClear.count; i++) {
                _onClear[i].Invoke();
            }
            
            _list.Clear();
        }
        
        public int IndexOf(T element) => _list.IndexOf(element);
        
        public bool Contains(T element) => _list.Contains(element);
        
        public void RemoveAt(int id) {
            T element = _list[id];
            
            if (_onRemove.isDirty) {
                _onRemove.Apply();
            }
            
            if (_onRemoveWithValue.isDirty) {
                _onRemoveWithValue.Apply();
            }
            
            for (int i = 0; i < _onRemove.count; i++) {
                _onRemove[i].Invoke();
            }
            
            for (int i = 0; i < _onRemoveWithValue.count; i++) {
                _onRemoveWithValue[i].Invoke(element);
            }
            
            _list.RemoveAt(id);
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
            foreach (T value in _list) {
                yield return value;
            }
        }
        
        public bool MoveNext() {
            _currentId++;
            return _currentId < _list.Count;
        }
        
        public void Reset() => _currentId = -1;
        
        public void Dispose() {
            Reset();
            
            foreach (T obj in _list) {
                if (obj is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
            
            _list = null;
            _onAdd.Clear();
            _onAddWithValue.Clear();
            _onRemove.Clear();
            _onRemoveWithValue.Clear();
            _onClear.Clear();
        }
    }
}