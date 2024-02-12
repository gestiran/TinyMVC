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
    public sealed class ObservedList<T> : IEnumerator<T> {
        public int count => _value.Count;
        public T Current => _value[_currentId];
        object IEnumerator.Current => _value[_currentId];
        
        internal List<Listener<T>> onAdd;
        internal List<Listener<T>> onRemove;
        internal List<Listener> onClear;

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
            onAdd = new List<Listener<T>>();
            onRemove = new List<Listener<T>>();
            onClear = new List<Listener>();
            _currentId = -1;
        }

        public T this[int index] {
            get => _value[index];
            set {
                for (int i = onRemove.Count - 1; i >= 0; i--) {
                    onRemove[i].Invoke(_value[index]);
                }
            
                _value[index] = value;

                for (int i = onAdd.Count - 1; i >= 0; i--) {
                    onAdd[i].Invoke(value);
                }
            }
        }

        public void Add([NotNull] params T[] values) {
            _value.AddRange(values);
            
            for (int i = onAdd.Count - 1; i >= 0; i--) {
                onAdd[i].Invoke(values);
            }
            
        #if UNITY_EDITOR
            if (_frameAddId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"ObservedList {type.Name} in {type.Namespace} add called twice in one frame!");
            }

            _frameAddId = ObservedTestUtility.frameId;
        #endif
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(Expanded = true), HorizontalGroup]
    #endif
        public void Add([NotNull] T value) {
            _value.Add(value);
            
            for (int i = onAdd.Count - 1; i >= 0; i--) {
                onAdd[i].Invoke(value);
            }
            
        #if UNITY_EDITOR
            if (_frameAddId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"ObservedList {type.Name} in {type.Namespace} add called twice in one frame!");
            }

            _frameAddId = ObservedTestUtility.frameId;
        #endif
        }

        public void Remove([NotNull] params T[] values) {
            for (int i = values.Length - 1; i >= 0; i--) {
                _value.Remove(values[i]);
            }
            
            for (int i = onRemove.Count - 1; i >= 0; i--) {
                onRemove[i].Invoke(values);
            }
            
        #if UNITY_EDITOR
            if (_frameRemoveId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"ObservedList {type.Name} in {type.Namespace} remove called twice in one frame!");
            }

            _frameRemoveId = ObservedTestUtility.frameId;
        #endif
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(Expanded = true), HorizontalGroup]
    #endif
        public void Remove([NotNull] T value) {
            _value.Remove(value);
            
            for (int i = onRemove.Count - 1; i >= 0; i--) {
                onRemove[i].Invoke(value);
            }
            
        #if UNITY_EDITOR
            if (_frameRemoveId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"ObservedList {type.Name} in {type.Namespace} remove called twice in one frame!");
            }

            _frameRemoveId = ObservedTestUtility.frameId;
        #endif
        }

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Clear() {
            _value.Clear();
            
            for (int i = onClear.Count - 1; i >= 0; i--) {
                onClear[i].Invoke();
            }
            
        #if UNITY_EDITOR
            if (_frameClearId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"ObservedList {type.Name} in {type.Namespace} clear called twice in one frame!");
            }

            _frameClearId = ObservedTestUtility.frameId;
        #endif
        }

        public int IndexOf(T element) => _value.IndexOf(element);
        
        public bool Contains(T element) => _value.Contains(element);
        
        public void RemoveAt(int id) {
            T element = _value[id];
            _value.RemoveAt(id);

            for (int i = onRemove.Count - 1; i >= 0; i--) {
                onRemove[i].Invoke(element);
            }
            
        #if UNITY_EDITOR
            if (_frameRemoveId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogWarning($"ObservedList {type.Name} in {type.Namespace} remove called twice in one frame!");
            }

            _frameRemoveId = ObservedTestUtility.frameId;
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
            onAdd = null;
            onRemove = null;
            onClear = null;
        }
    }
}