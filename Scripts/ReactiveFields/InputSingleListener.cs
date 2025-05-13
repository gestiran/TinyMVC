using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.ReactiveFields {
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputSingleListener : IUnload {
        private readonly List<Func<bool>> _listeners;
        
        public InputSingleListener() => _listeners = new List<Func<bool>>(16);
        
        public InputSingleListener(Func<bool> action) : this() => AddListener(action);
        
        public InputSingleListener(Func<bool> action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send() => _listeners.Invoke();
        
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