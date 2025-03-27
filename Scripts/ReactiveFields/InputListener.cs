using System.Collections.Generic;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.ReactiveFields {
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener : IUnload {
        internal readonly int id;
        
        public InputListener() => id = Observed.globalId++;
        
        public InputListener(ActionListener action) : this() => this.AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => this.AddListener(action, unload);
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send() {
            if (Listeners.pool.TryGetValue(id, out List<ActionListener> listeners)) {
                listeners.Invoke();
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => this.RemoveListeners();
    }
    
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T> : IUnload {
        internal readonly int id;
        
        public InputListener() => id = Observed.globalId++;
        
        public InputListener(ActionListener action) : this() => this.AddListener(action);
        
        public InputListener(ActionListener<T> action) : this() => this.AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => this.AddListener(action, unload);
        
        public InputListener(ActionListener<T> action, UnloadPool unload) : this() => this.AddListener(action, unload);
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send(T data = default) {
            if (Listeners.pool.TryGetValue(id, out List<ActionListener> listeners)) {
                listeners.Invoke();
            }
            
            if (Listeners<T>.pool.TryGetValue(id, out List<ActionListener<T>> valueListeners)) {
                valueListeners.Invoke(data);
            }
        }
        
        public void Send(params T[] data) {
            if (Listeners.pool.TryGetValue(id, out List<ActionListener> listeners)) {
                listeners.Invoke();
            }
            
            if (Listeners<T>.pool.TryGetValue(id, out List<ActionListener<T>> valueListeners)) {
                valueListeners.Invoke(data);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => this.RemoveListeners();
    }
    
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2> : IUnload {
        internal readonly int id;
        
        public InputListener() => id = Observed.globalId++;
        
        public InputListener(ActionListener action) : this() => this.AddListener(action);
        
        public InputListener(ActionListener<T1, T2> action) : this() => this.AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => this.AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2> action, UnloadPool unload) : this() => this.AddListener(action, unload);
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 data1 = default, T2 data2 = default) {
            if (Listeners.pool.TryGetValue(id, out List<ActionListener> listeners)) {
                listeners.Invoke();
            }
            
            if (Listeners<T1, T2>.pool.TryGetValue(id, out List<ActionListener<T1, T2>> valueListeners)) {
                valueListeners.Invoke(data1, data2);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => this.RemoveListeners();
    }
    
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2, T3> : IUnload {
        internal readonly int id;
        
        public InputListener() => id = Observed.globalId++;
        
        public InputListener(ActionListener action) : this() => this.AddListener(action);
        
        public InputListener(ActionListener<T1, T2, T3> action) : this() => this.AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => this.AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2, T3> action, UnloadPool unload) : this() => this.AddListener(action, unload);
        
    #if UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 data1 = default, T2 data2 = default, T3 data3 = default) {
            if (Listeners.pool.TryGetValue(id, out List<ActionListener> listeners)) {
                listeners.Invoke();
            }
            
            if (Listeners<T1, T2, T3>.pool.TryGetValue(id, out List<ActionListener<T1, T2, T3>> valueListeners)) {
                valueListeners.Invoke(data1, data2, data3);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => this.RemoveListeners();
    }
}