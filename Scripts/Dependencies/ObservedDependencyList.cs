using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TinyMVC.Loop;
using TinyMVC.Utilities.Async;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class ObservedDependencyList<T> : IEnumerable<T>, IEnumerator<T>, IDependency where T : IDependency {
        public int count => _value.Count;
        public T Current => _value[_currentId];
        object IEnumerator.Current => _value[_currentId];

        private readonly List<Action> _onAdd;
        private readonly List<Action<T>> _onAddWithValue;
        private readonly List<Action> _onRemove;
        private readonly List<Action<T>> _onRemoveWithValue;
        private readonly List<Action> _onClear;

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, HideLabel, ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
    #endif
        private List<T> _value;
        
        private int _currentId;
        private bool _lock;
        
        private const int _CAPACITY = 16;
        private const int _ASYNC_ANR_MS = 64;
        
        public ObservedDependencyList() : this(new List<T>()) { }
        
        public ObservedDependencyList(int capacity) : this(new List<T>(capacity)) { }

        public ObservedDependencyList(T[] value) : this(value.ToList()) { }
        
        public ObservedDependencyList([NotNull] DependencyPool<T> pool) {
            _value = new List<T>(pool.length);
            
            for (int valueId = 0; valueId < pool.length; valueId++) {
                _value.Add(pool[valueId]);
            }
            
            _onAdd = new List<Action>(_CAPACITY);
            _onAddWithValue = new List<Action<T>>(_CAPACITY);
            _onRemove = new List<Action>(_CAPACITY);
            _onRemoveWithValue = new List<Action<T>>(_CAPACITY);
            _onClear = new List<Action>(_CAPACITY);
            _currentId = -1;
        }
        
        public ObservedDependencyList([NotNull] params DependencyPool<T>[] pools) {
            int length = 0;

            for (int poolId = 0; poolId < pools.Length; poolId++) {
                length += pools[poolId].length;
            }

            _value = new List<T>(length);
            
            for (int poolId = 0; poolId < pools.Length; poolId++) {
                _value.AddRange(pools[poolId]);
            }
            
            _onAdd = new List<Action>(_CAPACITY);
            _onAddWithValue = new List<Action<T>>(_CAPACITY);
            _onRemove = new List<Action>(_CAPACITY);
            _onRemoveWithValue = new List<Action<T>>(_CAPACITY);
            _onClear = new List<Action>(_CAPACITY);
            _currentId = -1;
        }
        
        public ObservedDependencyList(List<T> value) {
            _value = value;
            _onAdd = new List<Action>(_CAPACITY);
            _onAddWithValue = new List<Action<T>>(_CAPACITY);
            _onRemove = new List<Action>(_CAPACITY);
            _onRemoveWithValue = new List<Action<T>>(_CAPACITY);
            _onClear = new List<Action>(_CAPACITY);
            _currentId = -1;
        }

        public T this[int index] {
            get => _value[index];
            set {
                Action listeners = null;
                Action<T> valueListeners = null;
            
                foreach (Action listener in _onRemove) {
                    listeners += listener;
                }
            
                foreach (Action<T> listener in _onRemoveWithValue) {
                    valueListeners += listener;
                }
            
                listeners?.Invoke();
                valueListeners?.Invoke(_value[index]);
                
                _value[index] = value;
                
                listeners = null;
                valueListeners = null;
            
                foreach (Action listener in _onAdd) {
                    listeners += listener;
                }
            
                foreach (Action<T> listener in _onAddWithValue) {
                    valueListeners += listener;
                }
            
                listeners?.Invoke();
                valueListeners?.Invoke(value);
            }
        }

        public void Add([NotNull] params T[] values) {
            _value.AddRange(values);
            
            Action listeners = null;
            Action<T> valueListeners = null;
            
            foreach (Action listener in _onAdd) {
                listeners += listener;
            }
            
            foreach (Action<T> listener in _onAddWithValue) {
                valueListeners += listener;
            }
            
            listeners?.Invoke();

            for (int i = 0; i < values.Length; i++) {
                valueListeners?.Invoke(values[i]);
            }
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(Expanded = true), HorizontalGroup]
    #endif
        public void Add([NotNull] T value) {
            _value.Add(value);
            
            Action listeners = null;
            Action<T> valueListeners = null;
            
            foreach (Action listener in _onAdd) {
                listeners += listener;
            }
            
            foreach (Action<T> listener in _onAddWithValue) {
                valueListeners += listener;
            }
            
            listeners?.Invoke();
            valueListeners?.Invoke(value);
        }

        public Task AddAsync([NotNull] params T[] values) => AddAsync(_ASYNC_ANR_MS, new AsyncCancellation(), values);
        
        public Task AddAsync(ICancellation cancellation, [NotNull] params T[] values) => AddAsync(_ASYNC_ANR_MS, cancellation, values);

        public async Task AddAsync(int anr, ICancellation cancellation, [NotNull] params T[] values) {
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
                    if (cancellation.isCancel) {
                        return;
                    }
                    
                    continue;
                }

                await Task.Yield();

                if (cancellation.isCancel) {
                    return;
                }
                
                now = DateTime.Now;
            }
            
            for (int i = _onAddWithValue.Count - 1; i >= 0; i--) {
                for (int j = 0; j < values.Length; j++) {
                    _onAddWithValue[i].Invoke(values[j]);
                }

                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.isCancel) {
                        return;
                    }
                    
                    continue;
                }

                await Task.Yield();

                if (cancellation.isCancel) {
                    return;
                }
                
                now = DateTime.Now;
            }

            _lock = false;
        }

        public  Task AddAsync([NotNull] T value) => AddAsync(_ASYNC_ANR_MS, new AsyncCancellation(), value);
        
        public  Task AddAsync(ICancellation cancellation, [NotNull] T value) => AddAsync(_ASYNC_ANR_MS, cancellation, value);

        public async Task AddAsync(int anr, ICancellation cancellation, [NotNull] T value) {
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
                    if (cancellation.isCancel) {
                        return;
                    }
                    
                    continue;
                }

                await Task.Yield();

                if (cancellation.isCancel) {
                    return;
                }
                
                now = DateTime.Now;
            }
            
            for (int i = _onAddWithValue.Count - 1; i >= 0; i--) {
                _onAddWithValue[i].Invoke(value);

                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.isCancel) {
                        return;
                    }
                    
                    continue;
                }

                await Task.Yield();

                if (cancellation.isCancel) {
                    return;
                }
                
                now = DateTime.Now;
            }

            _lock = false;
        }

        public void Remove([NotNull] params T[] values) {
            for (int i = values.Length - 1; i >= 0; i--) {
                _value.Remove(values[i]);
            }
            
            Action listeners = null;
            Action<T> valueListeners = null;
            
            foreach (Action listener in _onRemove) {
                listeners += listener;
            }
            
            foreach (Action<T> listener in _onRemoveWithValue) {
                valueListeners += listener;
            }
            
            listeners?.Invoke();

            for (int i = 0; i < values.Length; i++) {
                valueListeners?.Invoke(values[i]);   
            }
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(Expanded = true), HorizontalGroup]
    #endif
        public void Remove([NotNull] T value) {
            _value.Remove(value);
            
            Action listeners = null;
            Action<T> valueListeners = null;
            
            foreach (Action listener in _onRemove) {
                listeners += listener;
            }
            
            foreach (Action<T> listener in _onRemoveWithValue) {
                valueListeners += listener;
            }
            
            listeners?.Invoke();
            valueListeners?.Invoke(value);
        }
        
        public Task RemoveAsync([NotNull] params T[] values) => RemoveAsync(_ASYNC_ANR_MS, new AsyncCancellation(), values);
        
        public Task RemoveAsync(ICancellation cancellation, [NotNull] params T[] values) => RemoveAsync(_ASYNC_ANR_MS, cancellation, values);
        
        public async Task RemoveAsync(int anr, ICancellation cancellation, [NotNull] params T[] values) {
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
                    if (cancellation.isCancel) {
                        return;
                    }
                    
                    continue;
                }

                await Task.Yield();
                
                if (cancellation.isCancel) {
                    return;
                }
                
                now = DateTime.Now;
            }
            
            for (int i = _onRemoveWithValue.Count - 1; i >= 0; i--) {
                for (int j = 0; j < values.Length; j++) {
                    _onRemoveWithValue[i].Invoke(values[j]);   
                }

                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.isCancel) {
                        return;
                    }
                    
                    continue;
                }

                await Task.Yield();
                
                if (cancellation.isCancel) {
                    return;
                }
                
                now = DateTime.Now;
            }

            _lock = false;
        }
        
        public  Task RemoveAsync([NotNull] T value) => RemoveAsync(_ASYNC_ANR_MS, new AsyncCancellation(), value);
        
        public  Task RemoveAsync(ICancellation cancellation, [NotNull] T value) => RemoveAsync(_ASYNC_ANR_MS, cancellation, value);

        public async Task RemoveAsync(int anr, ICancellation cancellation, [NotNull] T value) {
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
                    if (cancellation.isCancel) {
                        return;
                    }
                    
                    continue;
                }

                await Task.Yield();
                
                if (cancellation.isCancel) {
                    return;
                }
                
                now = DateTime.Now;
            }
            
            for (int i = _onRemoveWithValue.Count - 1; i >= 0; i--) {
                _onRemoveWithValue[i].Invoke(value);

                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.isCancel) {
                        return;
                    }
                    
                    continue;
                }

                await Task.Yield();
                
                if (cancellation.isCancel) {
                    return;
                }
                
                now = DateTime.Now;
            }

            _lock = false;
        }

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Clear() {
            _value.Clear();
            
            Action listeners = null;
            
            foreach (Action listener in _onClear) {
                listeners += listener;
            }
            
            listeners?.Invoke();
        }

        public int IndexOf(T element) => _value.IndexOf(element);
        
        public bool Contains(T element) => _value.Contains(element);
        
        public void RemoveAt(int id) {
            T element = _value[id];
            _value.RemoveAt(id);
            
            Action listeners = null;
            Action<T> valueListeners = null;
            
            foreach (Action listener in _onRemove) {
                listeners += listener;
            }
            
            foreach (Action<T> listener in _onRemoveWithValue) {
                valueListeners += listener;
            }
            
            listeners?.Invoke();
            valueListeners?.Invoke(element);
        }
        
        public void AddOnAddListener(Action listener) => _onAdd.Add(listener);

        public void AddOnAddListener(Action listener, UnloadPool unload) {
            _onAdd.Add(listener);
            unload.Add(new UnloadAction(() => _onAdd.Remove(listener)));
        }
        
        public void AddOnAddListener(Action<T> listener) => _onAddWithValue.Add(listener);

        public void AddOnAddListener(Action<T> listener, UnloadPool unload) {
            _onAddWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onAddWithValue.Remove(listener)));
        }
        
        public void RemoveOnAddListener(Action listener) => _onAdd.Remove(listener);

        public void RemoveOnAddListener(Action<T> listener) => _onAddWithValue.Remove(listener);
        
        public void AddOnRemoveListener(Action listener) => _onRemove.Add(listener);

        public void AddOnRemoveListener(Action listener, UnloadPool unload) {
            _onRemove.Add(listener);
            unload.Add(new UnloadAction(() => _onRemove.Remove(listener)));
        }
        
        public void AddOnRemoveListener(Action<T> listener) => _onRemoveWithValue.Add(listener);

        public void AddOnRemoveListener(Action<T> listener, UnloadPool unload) {
            _onRemoveWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onRemoveWithValue.Remove(listener)));
        }
        
        public void RemoveOnRemoveListener(Action listener) => _onRemove.Remove(listener);

        public void RemoveOnRemoveListener(Action<T> listener) => _onRemoveWithValue.Remove(listener);
        
        public void AddOnClearListener(Action listener) => _onClear.Add(listener);

        public void AddOnClearListener(Action listener, UnloadPool unload) {
            _onClear.Add(listener);
            unload.Add(new UnloadAction(() => _onClear.Remove(listener)));
        }
        
        public void RemoveOnClearListener(Action listener) => _onClear.Remove(listener);
        
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