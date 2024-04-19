using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TinyMVC.ReactiveFields.Extensions;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class ObservedList<T> : IEnumerable<T>, IEnumerator<T>, IObservedList<T> {
        public int count => _value.Count;
        public T Current => _value[_currentId];
        object IEnumerator.Current => _value[_currentId];

        List<Listener<T>> IObservedList<T>.onAdd => _onAdd;

        List<Listener<T>> IObservedList<T>.onRemove => _onRemove;

        List<Listener> IObservedList.onClear => _onClear;

        private List<Listener<T>> _onAdd;
        private List<Listener<T>> _onRemove;
        private List<Listener> _onClear;

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, HideLabel, ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
    #endif
        private List<T> _value;

        private int _currentId;
        private bool _lock;

    #if PERFORMANCE_DEBUG
        private uint _frameAddId;
        private uint _frameRemoveId;
        private uint _frameClearId;
    #endif

        private const int _ASYNC_ANR_MS = 64;

        public ObservedList() : this(new List<T>()) { }

        public ObservedList(T[] value) : this(value.ToList()) { }

        public ObservedList(List<T> value) {
            _value = value;
            _onAdd = new List<Listener<T>>();
            _onRemove = new List<Listener<T>>();
            _onClear = new List<Listener>();
            _currentId = -1;
        }

        public T this[int index] {
            get => _value[index];
            set {
                _onRemove.Invoke(listener => listener.Invoke(_value[index]));

            #if PERFORMANCE_DEBUG
                _frameRemoveId = UpdateFrame(_frameRemoveId, nameof(Remove));
            #endif

                _value[index] = value;

                _onAdd.Invoke(listener => listener.Invoke(value));

            #if PERFORMANCE_DEBUG
                _frameAddId = UpdateFrame(_frameAddId, nameof(Add));
            #endif
            }
        }

        public void Add([NotNull] params T[] values) {
            _value.AddRange(values);
            _onAdd.Invoke(listener => listener.Invoke(values));

        #if PERFORMANCE_DEBUG
            _frameAddId = UpdateFrame(_frameAddId, nameof(Add));
        #endif
        }

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(Expanded = true), HorizontalGroup]
    #endif
        public void Add([NotNull] T value) {
            _value.Add(value);
            _onAdd.Invoke(listener => listener.Invoke(value));

        #if PERFORMANCE_DEBUG
            _frameAddId = UpdateFrame(_frameAddId, nameof(Add));
        #endif
        }

        public async Task AddAsync([NotNull] params T[] values) {
            if (_lock) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }

            _lock = true;
            _value.AddRange(values);
            DateTime now = DateTime.Now;

            for (int i = _onAdd.Count - 1; i >= 0; i--) {
                _onAdd[i].Invoke(values);

                if (DateTime.Now.Subtract(now).TotalMilliseconds < _ASYNC_ANR_MS) {
                    continue;
                }

                await Task.Yield();
                now = DateTime.Now;
            }

            _lock = false;

        #if PERFORMANCE_DEBUG
            _frameAddId = UpdateFrame(_frameAddId, nameof(Add));
        #endif
        }

        public async Task AddAsync([NotNull] T value) {
            if (_lock) {
            #if PERFORMANCE_DEBUG
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }

            _lock = true;
            _value.Add(value);
            DateTime now = DateTime.Now;

            for (int i = _onAdd.Count - 1; i >= 0; i--) {
                _onAdd[i].Invoke(value);

                if (DateTime.Now.Subtract(now).TotalMilliseconds < _ASYNC_ANR_MS) {
                    continue;
                }

                await Task.Yield();
                now = DateTime.Now;
            }

            _lock = false;

        #if PERFORMANCE_DEBUG
            _frameAddId = UpdateFrame(_frameAddId, nameof(Add));
        #endif
        }

        public void Remove([NotNull] params T[] values) {
            for (int i = values.Length - 1; i >= 0; i--) {
                _value.Remove(values[i]);
            }

            _onRemove.Invoke(listener => listener.Invoke(values));

        #if PERFORMANCE_DEBUG
            _frameRemoveId = UpdateFrame(_frameRemoveId, nameof(Remove));
        #endif
        }

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(Expanded = true), HorizontalGroup]
    #endif
        public void Remove([NotNull] T value) {
            _value.Remove(value);
            _onRemove.Invoke(listener => listener.Invoke(value));

        #if PERFORMANCE_DEBUG
            _frameRemoveId = UpdateFrame(_frameRemoveId, nameof(Remove));
        #endif
        }

        public async Task RemoveAsync([NotNull] params T[] values) {
            if (_lock) {
            #if PERFORMANCE_DEBUG
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
                _onRemove[i].Invoke(values);

                if (DateTime.Now.Subtract(now).TotalMilliseconds < _ASYNC_ANR_MS) {
                    continue;
                }

                await Task.Yield();
                now = DateTime.Now;
            }

            _lock = false;

        #if PERFORMANCE_DEBUG
            _frameRemoveId = UpdateFrame(_frameRemoveId, nameof(Remove));
        #endif
        }

        public async Task RemoveAsync([NotNull] T value) {
            if (_lock) {
            #if PERFORMANCE_DEBUG
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }

            _lock = true;
            _value.Remove(value);
            DateTime now = DateTime.Now;

            for (int i = _onRemove.Count - 1; i >= 0; i--) {
                _onRemove[i].Invoke(value);

                if (DateTime.Now.Subtract(now).TotalMilliseconds < _ASYNC_ANR_MS) {
                    continue;
                }

                await Task.Yield();
                now = DateTime.Now;
            }

            _lock = false;

        #if PERFORMANCE_DEBUG
            _frameRemoveId = UpdateFrame(_frameRemoveId, nameof(Remove));
        #endif
        }

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Clear() {
            _value.Clear();
            _onClear.Invoke(listener => listener.Invoke());

        #if PERFORMANCE_DEBUG
            _frameClearId = UpdateFrame(_frameClearId, nameof(Clear));
        #endif
        }

        public int IndexOf(T element) => _value.IndexOf(element);

        public bool Contains(T element) => _value.Contains(element);

        public void RemoveAt(int id) {
            T element = _value[id];
            _value.RemoveAt(id);
            _onRemove.Invoke(listener => listener.Invoke(element));

        #if PERFORMANCE_DEBUG
            _frameRemoveId = UpdateFrame(_frameRemoveId, nameof(Remove));
        #endif
        }

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
            _onAdd = null;
            _onRemove = null;
            _onClear = null;
        }

    #if PERFORMANCE_DEBUG
        private uint UpdateFrame(uint frame, string action) {
            if (frame == ObservedUtility.frameId) {
                Type type = typeof(T);
                Debug.LogWarning($"ObservedList {type.Name} in {type.Namespace} {action} called twice in one frame!");
            }

            return ObservedUtility.frameId;
        }

    #endif
    }
}