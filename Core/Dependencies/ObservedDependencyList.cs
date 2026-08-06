// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TinyReactive;
using TinyReactive.Fields;

namespace TinyMVC.Dependencies {
    public sealed class ObservedDependencyList<T> : IList<T>, IEnumerator<T>, IDependency where T : IDependency {
        public int Count => list.Count;
        public bool IsReadOnly => false;
        public T Current => list[_currentId];
        object IEnumerator.Current => list[_currentId];
        
        internal List<T> list;
        
        private int _currentId;
        private bool _lock;
        
        /// <summary> Unique identifier automatically assigned to this instance. </summary>
        internal readonly int id;
        
        internal readonly LazyList<ActionListener> onAdd;
        internal readonly LazyList<ActionListener<T>> onAddWithValue;
        internal readonly LazyList<ActionListener> onRemove;
        internal readonly LazyList<ActionListener<T>> onRemoveWithValue;
        internal readonly LazyList<ActionListener> onClear;
        
        public ObservedDependencyList(int capacity = Observed.CAPACITY) : this(new List<T>(), capacity) { }
        
        public ObservedDependencyList(T[] value, int capacity = Observed.CAPACITY) : this(value.ToList(), capacity) { }
        
        public ObservedDependencyList([NotNull] DependencyPool<T> pool, int capacity = Observed.CAPACITY) {
            list = new List<T>(pool.length);
            
            for (int valueId = 0; valueId < pool.length; valueId++) {
                list.Add(pool[valueId]);
            }
            
            id = Observed.GetID();
            onAdd = new LazyList<ActionListener>(capacity);
            onAddWithValue = new LazyList<ActionListener<T>>(capacity);
            onRemove = new LazyList<ActionListener>(capacity);
            onRemoveWithValue = new LazyList<ActionListener<T>>(capacity);
            onClear = new LazyList<ActionListener>(capacity);
            _currentId = -1;
        }
        
        public ObservedDependencyList([NotNull] params DependencyPool<T>[] pools) {
            int length = 0;
            
            for (int poolId = 0; poolId < pools.Length; poolId++) {
                length += pools[poolId].length;
            }
            
            list = new List<T>(length);
            
            for (int poolId = 0; poolId < pools.Length; poolId++) {
                list.AddRange(pools[poolId]);
            }
            
            id = Observed.GetID();
            onAdd = new LazyList<ActionListener>(Observed.CAPACITY);
            onAddWithValue = new LazyList<ActionListener<T>>(Observed.CAPACITY);
            onRemove = new LazyList<ActionListener>(Observed.CAPACITY);
            onRemoveWithValue = new LazyList<ActionListener<T>>(Observed.CAPACITY);
            onClear = new LazyList<ActionListener>(Observed.CAPACITY);
            _currentId = -1;
        }
        
        public ObservedDependencyList(List<T> value, int capacity = Observed.CAPACITY) {
            list = value;
            id = Observed.GetID();
            onAdd = new LazyList<ActionListener>(capacity);
            onAddWithValue = new LazyList<ActionListener<T>>(capacity);
            onRemove = new LazyList<ActionListener>(capacity);
            onRemoveWithValue = new LazyList<ActionListener<T>>(capacity);
            onClear = new LazyList<ActionListener>(capacity);
            _currentId = -1;
        }
        
        public T this[int index] {
            get => list[index];
            set {
                if (onRemove.isDirty) {
                    onRemove.Apply();
                }
                
                if (onRemoveWithValue.isDirty) {
                    onRemoveWithValue.Apply();
                }
                
                for (int i = 0; i < onRemove.Count; i++) {
                    onRemove[i].Invoke();
                }
                
                for (int i = 0; i < onRemoveWithValue.Count; i++) {
                    onRemoveWithValue[i].Invoke(list[index]);
                }
                
                list[index] = value;
                
                if (onAdd.isDirty) {
                    onAdd.Apply();
                }
                
                if (onAddWithValue.isDirty) {
                    onAddWithValue.Apply();
                }
                
                for (int i = 0; i < onAdd.Count; i++) {
                    onAdd[i].Invoke();
                }
                
                for (int i = 0; i < onAddWithValue.Count; i++) {
                    onAddWithValue[i].Invoke(value);
                }
            }
        }
        
        [Obsolete("Can`t use without parameters!", true)]
        public void Add() {
            // Do nothing
        }
        
        public void Add([NotNull] params T[] values) {
            list.AddRange(values);
            
            if (onAdd.isDirty) {
                onAdd.Apply();
            }
            
            if (onAddWithValue.isDirty) {
                onAddWithValue.Apply();
            }
            
            for (int i = 0; i < onAdd.Count; i++) {
                onAdd[i].Invoke();
            }
            
            for (int valueId = 0; valueId < values.Length; valueId++) {
                T value = values[valueId];
                
                for (int i = 0; i < onAddWithValue.Count; i++) {
                    onAddWithValue[i].Invoke(value);
                }
            }
        }
        
        public void Add([NotNull] T value) {
            list.Add(value);
            
            if (onAdd.isDirty) {
                onAdd.Apply();
            }
            
            if (onAddWithValue.isDirty) {
                onAddWithValue.Apply();
            }
            
            for (int i = 0; i < onAdd.Count; i++) {
                onAdd[i].Invoke();
            }
            
            for (int i = 0; i < onAddWithValue.Count; i++) {
                onAddWithValue[i].Invoke(value);
            }
        }
        
        [Obsolete("Can`t use without parameters!", true)]
        public void Remove() {
            // Do nothing
        }
        
        public void Remove([NotNull] params T[] values) {
            if (onRemove.isDirty) {
                onRemove.Apply();
            }
            
            if (onRemoveWithValue.isDirty) {
                onRemoveWithValue.Apply();
            }
            
            foreach (T value in values) {
                int index = list.IndexOf(value);
                
                if (index >= 0) {
                    for (int i = 0; i < onRemove.Count; i++) {
                        onRemove[i].Invoke();
                    }
                    
                    for (int i = 0; i < onRemoveWithValue.Count; i++) {
                        onRemoveWithValue[i].Invoke(value);
                    }
                    
                    list.RemoveAt(index);
                }
            }
        }
        
        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        
        public bool Remove([NotNull] T value) {
            int index = list.IndexOf(value);
            
            if (index >= 0) {
                if (onRemove.isDirty) {
                    onRemove.Apply();
                }
                
                if (onRemoveWithValue.isDirty) {
                    onRemoveWithValue.Apply();
                }
                
                for (int i = 0; i < onRemove.Count; i++) {
                    onRemove[i].Invoke();
                }
                
                for (int i = 0; i < onRemoveWithValue.Count; i++) {
                    onRemoveWithValue[i].Invoke(value);
                }
                
                list.RemoveAt(index);
                
                return true;
            }
            
            return false;
        }
        
        public void Clear() {
            if (onClear.isDirty) {
                onClear.Apply();
            }
            
            for (int i = 0; i < onClear.Count; i++) {
                onClear[i].Invoke();
            }
            
            list.Clear();
        }
        
        public int IndexOf(T element) => list.IndexOf(element);
        
        public void Insert(int index, T item) {
            list.Insert(index, item);
            
            if (onAdd.isDirty) {
                onAdd.Apply();
            }
            
            if (onAddWithValue.isDirty) {
                onAddWithValue.Apply();
            }
            
            for (int i = 0; i < onAdd.Count; i++) {
                onAdd[i].Invoke();
            }
            
            for (int i = 0; i < onAddWithValue.Count; i++) {
                onAddWithValue[i].Invoke(item);
            }
        }
        
        public bool Contains(T element) => list.Contains(element);
        
        public void RemoveAt(int index) {
            T element = list[index];
            
            if (onRemove.isDirty) {
                onRemove.Apply();
            }
            
            if (onRemoveWithValue.isDirty) {
                onRemoveWithValue.Apply();
            }
            
            for (int i = 0; i < onRemove.Count; i++) {
                onRemove[i].Invoke();
            }
            
            for (int i = 0; i < onRemoveWithValue.Count; i++) {
                onRemoveWithValue[i].Invoke(element);
            }
            
            list.RemoveAt(index);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnAddListener(ActionListener listener) {
            onAdd.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnAddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onAdd.Add(listener);
            unload.Add(new UnloadAction(() => onAdd.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnAddListener(ActionListener<T> listener) {
            onAddWithValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnAddListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            onAddWithValue.Add(listener);
            unload.Add(new UnloadAction(() => onAddWithValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnAddListenerValue<TV>(ActionListener listener, IUnloadLink unload) where TV : T {
            AddOnAddListener(v =>
            {
                if (v is TV) {
                    listener.Invoke();
                }
            }, unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnAddListenerValue<TV>(ActionListener<TV> listener, IUnloadLink unload) where TV : T {
            AddOnAddListener(v =>
            {
                if (v is TV target) {
                    listener.Invoke(target);
                }
            }, unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> RemoveOnAddListener(ActionListener listener) {
            onAdd.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> RemoveOnAddListener(ActionListener<T> listener) {
            onAddWithValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnRemoveListener(ActionListener listener) {
            onRemove.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnRemoveListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onRemove.Add(listener);
            unload.Add(new UnloadAction(() => onRemove.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnRemoveListener(ActionListener<T> listener) {
            onRemoveWithValue.Add(listener);
            return this;
        }
        
        public ObservedDependencyList<T> AddOnRemoveListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            onRemoveWithValue.Add(listener);
            unload.Add(new UnloadAction(() => onRemoveWithValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnRemoveListenerValue<TV>(ActionListener listener, IUnloadLink unload) where TV : T {
            AddOnRemoveListener(v =>
            {
                if (v is TV) {
                    listener.Invoke();
                }
            }, unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnRemoveListenerValue<TV>(ActionListener<TV> listener, IUnloadLink unload) where TV : T {
            AddOnRemoveListener(v =>
            {
                if (v is TV target) {
                    listener.Invoke(target);
                }
            }, unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> RemoveOnRemoveListener(ActionListener listener) {
            onRemove.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> RemoveOnRemoveListener(ActionListener<T> listener) {
            onRemoveWithValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnClearListener(ActionListener listener) {
            onClear.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnClearListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onClear.Add(listener);
            unload.Add(new UnloadAction(() => onClear.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> RemoveOnClearListener(ActionListener listener) {
            onClear.Remove(listener);
            return this;
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public IEnumerator<T> GetEnumerator() {
            foreach (T value in list) {
                yield return value;
            }
        }
        
        public bool MoveNext() {
            _currentId++;
            return _currentId < list.Count;
        }
        
        public void Reset() => _currentId = -1;
        
        public void Dispose() {
            Reset();
            
            foreach (T obj in list) {
                if (obj is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
            
            list = null;
            onAdd.Clear();
            onAddWithValue.Clear();
            onRemove.Clear();
            onRemoveWithValue.Clear();
            onClear.Clear();
        }
    }
}