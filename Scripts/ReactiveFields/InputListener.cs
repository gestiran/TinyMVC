﻿// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
#if ODIN_INSPECTOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener : IUnload {
        private readonly int _id;
        private readonly List<ActionListener> _listeners;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new List<ActionListener>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if ODIN_INSPECTOR
        [Button]
    #endif
        public void Send() => _listeners.Invoke();
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener);
        
    #endregion
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => _listeners.Clear();
        
        public override int GetHashCode() => _id;
    }
    
#if ODIN_INSPECTOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T> : IUnload {
        private readonly int _id;
        private readonly List<ActionListener> _listeners;
        private readonly List<ActionListener<T>> _listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new List<ActionListener>(capacity);
            _listenersValue = new List<ActionListener<T>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T> action, UnloadPool unload) : this() => AddListener(action, unload);
        
        [Obsolete("Can't use without parameters!", true)]
        public void Send() {
            // Do nothing
        }
        
    #if ODIN_INSPECTOR
        [Button]
    #endif
        public void Send(T data) {
            _listeners.Invoke();
            _listenersValue.Invoke(data);
        }
        
        public void Send(params T[] data) {
            _listeners.Invoke();
            _listenersValue.Invoke(data);
        }
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener) => _listenersValue.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T> listener) => _listenersValue.Remove(listener);
        
    #endregion Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
    
#if ODIN_INSPECTOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2> : IUnload {
        private readonly int _id;
        private readonly List<ActionListener> _listeners;
        private readonly List<ActionListener<T1, T2>> _listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new List<ActionListener>(capacity);
            _listenersValue = new List<ActionListener<T1, T2>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T1, T2> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2> action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if ODIN_INSPECTOR
        [Button]
    #endif
        public void Send(T1 data1, T2 data2) {
            _listeners.Invoke();
            _listenersValue.Invoke(data1, data2);
        }
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2> listener) => _listenersValue.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T1, T2> listener) => _listenersValue.Remove(listener);
        
    #endregion
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
    
#if ODIN_INSPECTOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2, T3> : IUnload {
        private readonly int _id;
        private readonly List<ActionListener> _listeners;
        private readonly List<ActionListener<T1, T2, T3>> _listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new List<ActionListener>(capacity);
            _listenersValue = new List<ActionListener<T1, T2, T3>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T1, T2, T3> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2, T3> action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if ODIN_INSPECTOR
        [Button]
    #endif
        public void Send(T1 data1, T2 data2, T3 data3) {
            _listeners.Invoke();
            _listenersValue.Invoke(data1, data2, data3);
        }
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2, T3> listener) => _listenersValue.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2, T3> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T1, T2, T3> listener) => _listenersValue.Remove(listener);
        
    #endregion
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
}