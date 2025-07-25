﻿// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies {
#if ODIN_INSPECTOR
    [HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class DependencyPool<T> : IEnumerable<T>, IEnumerator<T>, IDependency where T : IDependency {
        public int length => _objects.Length;
        public T Current => _objects[_currentId];
        object IEnumerator.Current => _objects[_currentId];
        
    #if ODIN_INSPECTOR
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, IsReadOnly = true, ListElementLabelName = "@ToString()"), Searchable]
        [ShowInInspector, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox, LabelText("@ToString()")]
    #endif
        private T[] _objects;
        
        private int _currentId;
        
        public DependencyPool(int count) {
            _objects = new T[count];
            _currentId = -1;
        }
        
        public DependencyPool([NotNull] params DependencyPool<T>[] pools) {
            int count = 0;
            
            for (int poolId = 0; poolId < pools.Length; poolId++) {
                count += pools[poolId].length;
            }
            
            _objects = new T[count];
            count = 0;
            
            for (int poolId = 0; poolId < pools.Length; poolId++) {
                Array.Copy(pools[poolId]._objects, 0, _objects, count, pools[poolId].length);
                count += pools[poolId].length;
            }
            
            _currentId = -1;
        }
        
        public DependencyPool([NotNull] List<T> pool) {
            _objects = pool.ToArray();
            _currentId = -1;
        }
        
        public DependencyPool([NotNull] params T[] pool) {
            _objects = pool;
            _currentId = -1;
        }
        
        public DependencyPool([NotNull] DependencyPool<T> pool) {
            _objects = new T[pool._objects.Length];
            Array.Copy(pool._objects, _objects, pool._objects.Length);
            _currentId = -1;
        }
        
        public T this[int index] {
            get => _objects[index];
            set => _objects[index] = value;
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public IEnumerator<T> GetEnumerator() {
            foreach (T value in _objects) {
                yield return value;
            }
        }
        
        public bool MoveNext() {
            _currentId++;
            
            return _currentId < _objects.Length;
        }
        
        public void Reset() => _currentId = -1;
        
        public void Dispose() {
            Reset();
            
            if (_objects.Length > 0 && _objects[0] is IDisposable) {
                foreach (T obj in _objects) {
                    (obj as IDisposable).Dispose();
                }
            }
            
            _objects = null;
        }
        
        public override string ToString() => $"DependencyPool<{typeof(T).Name}>";
    }
}