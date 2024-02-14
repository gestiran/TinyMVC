using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class ObservedList<T> : IEnumerator<T>, IObservedList<T> {
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
        
    #if UNITY_EDITOR
        private uint _frameAddId;
        private uint _frameRemoveId;
        private uint _frameClearId;
    #endif

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
                for (int i = _onRemove.Count - 1; i >= 0; i--) {
                    _onRemove[i].Invoke(_value[index]);
                }
            
            #if UNITY_EDITOR
                _frameRemoveId = UpdateFrame(_frameRemoveId, nameof(Remove));
            #endif
                
                _value[index] = value;

                for (int i = _onAdd.Count - 1; i >= 0; i--) {
                    _onAdd[i].Invoke(value);
                }
                
            #if UNITY_EDITOR
                _frameAddId = UpdateFrame(_frameAddId, nameof(Add));
            #endif
            }
        }

        public void Add([NotNull] params T[] values) {
            _value.AddRange(values);
            
            for (int i = _onAdd.Count - 1; i >= 0; i--) {
                _onAdd[i].Invoke(values);
            }
            
        #if UNITY_EDITOR
            _frameAddId = UpdateFrame(_frameAddId, nameof(Add));
        #endif
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(Expanded = true), HorizontalGroup]
    #endif
        public void Add([NotNull] T value) {
            _value.Add(value);
            
            for (int i = _onAdd.Count - 1; i >= 0; i--) {
                _onAdd[i].Invoke(value);
            }
            
        #if UNITY_EDITOR
            _frameAddId = UpdateFrame(_frameAddId, nameof(Add));
        #endif
        }

        public void Remove([NotNull] params T[] values) {
            for (int i = values.Length - 1; i >= 0; i--) {
                _value.Remove(values[i]);
            }
            
            for (int i = _onRemove.Count - 1; i >= 0; i--) {
                _onRemove[i].Invoke(values);
            }
            
        #if UNITY_EDITOR
            _frameRemoveId = UpdateFrame(_frameRemoveId, nameof(Remove));
        #endif
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(Expanded = true), HorizontalGroup]
    #endif
        public void Remove([NotNull] T value) {
            _value.Remove(value);
            
            for (int i = _onRemove.Count - 1; i >= 0; i--) {
                _onRemove[i].Invoke(value);
            }
            
        #if UNITY_EDITOR
            _frameRemoveId = UpdateFrame(_frameRemoveId, nameof(Remove));
        #endif
        }

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Clear() {
            _value.Clear();
            
            for (int i = _onClear.Count - 1; i >= 0; i--) {
                _onClear[i].Invoke();
            }
            
        #if UNITY_EDITOR
            _frameClearId = UpdateFrame(_frameClearId, nameof(Clear));
        #endif
        }

        public int IndexOf(T element) => _value.IndexOf(element);
        
        public bool Contains(T element) => _value.Contains(element);
        
        public void RemoveAt(int id) {
            T element = _value[id];
            _value.RemoveAt(id);

            for (int i = _onRemove.Count - 1; i >= 0; i--) {
                _onRemove[i].Invoke(element);
            }
            
        #if UNITY_EDITOR
            _frameRemoveId = UpdateFrame(_frameRemoveId, nameof(Remove));
        #endif
        }
        
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
        
    #if UNITY_EDITOR || DEVELOPMENT_BUILD

        private uint UpdateFrame(uint frame, string action) {
            if (frame == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogError($"ObservedList {type.Name} in {type.Namespace} {action} called twice in one frame!");
            }

            return ObservedTestUtility.frameId;
        }
        
    #endif
    }
}