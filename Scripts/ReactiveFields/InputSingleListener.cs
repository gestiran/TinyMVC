// Copyright (c) 2023 Derek Sliman
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
    public sealed class InputSingleListener : IUnload {
        private readonly List<Func<bool>> _listeners;
        
        public InputSingleListener() => _listeners = new List<Func<bool>>(Observed.CAPACITY);
        
        public InputSingleListener(Func<bool> action) : this() => AddListener(action);
        
        public InputSingleListener(Func<bool> action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if ODIN_INSPECTOR
        [Button]
    #endif
        public void Send(bool expectedResult = true) => _listeners.InvokeAny(expectedResult);
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Func<bool> listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(Func<bool> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(Func<bool> listener) => _listeners.Remove(listener);
        
    #endregion
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => _listeners.Clear();
    }
}