using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class ObservedDependencyList<T> : IEnumerator<T>, IDependency where T : IDependency {
        public T Current => _value[_currentId];
        object IEnumerator.Current => _value[_currentId];
        public int length => _value.Count;
        
        private List<MultipleListener<T>> _onAdd;
        private List<MultipleListener<T>> _onRemove;
        private List<Action> _onClear;

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

        public ObservedDependencyList() : this(new List<T>()) { }
        
        public ObservedDependencyList(int capacity) : this(new List<T>(capacity)) { }

        public ObservedDependencyList(T[] value) : this(value.ToList()) { }
        
        public ObservedDependencyList(List<T> value) {
            _value = value;
            _onAdd = new List<MultipleListener<T>>();
            _onRemove = new List<MultipleListener<T>>();
            _onClear = new List<Action>();
            _currentId = -1;
        }

        public T this[int key] {
            get => _value[key];
            set => _value[key] = value;
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(Expanded = true), HorizontalGroup]
    #endif
        public void Add([NotNull] params T[] values) {
            _value.AddRange(values);
            
            for (int i = _onAdd.Count - 1; i >= 0; i--) {
                _onAdd[i].Invoke(values);
            }
            
        #if UNITY_EDITOR
            if (_frameAddId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogError($"ObservedList {type.Name} in {type.Namespace} add called twice in one frame!");
            }

            _frameAddId = ObservedTestUtility.frameId;
        #endif
        }

    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(Expanded = true), HorizontalGroup]
    #endif
        public void Remove([NotNull] params T[] values) {
            for (int i = 0; i < values.Length; i++) {
                _value.Remove(values[i]);
            }
            
            for (int i = _onRemove.Count - 1; i >= 0; i--) {
                _onRemove[i].Invoke(values);
            }
            
        #if UNITY_EDITOR
            if (_frameRemoveId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogError($"ObservedList {type.Name} in {type.Namespace} remove called twice in one frame!");
            }

            _frameRemoveId = ObservedTestUtility.frameId;
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
            if (_frameClearId == ObservedTestUtility.frameId) {
                Type type = typeof(T);
                UnityEngine.Debug.LogError($"ObservedList {type.Name} in {type.Namespace} clear called twice in one frame!");
            }

            _frameClearId = ObservedTestUtility.frameId;
        #endif
        }

        public IEnumerator<T> GetEnumerator() {
            foreach (T value in _value) {
                yield return value;
            }
        }

        public void AddOnAddListener(MultipleListener<T> listener) => _onAdd.Add(listener);
        
        public void RemoveOnAddListener(MultipleListener<T> listener) => _onAdd.Remove(listener);
        
        public void AddOnRemoveListener(MultipleListener<T> listener) => _onRemove.Add(listener);
        
        public void RemoveOnRemoveListener(MultipleListener<T> listener) => _onRemove.Remove(listener);
        
        public void AddOnClearListener(Action listener) => _onClear.Add(listener);
        
        public void RemoveOnClearListener(Action listener) => _onClear.Remove(listener);

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
    }
}