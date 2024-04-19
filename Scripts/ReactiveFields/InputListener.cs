using System.Collections.Generic;
using JetBrains.Annotations;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

using NotNullSystem = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace TinyMVC.ReactiveFields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener : IUnload {
        internal List<Listener> listeners = new List<Listener>();
        
    #if UNITY_EDITOR
        private uint _frameId;
    #endif
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send() {
            listeners.Invoke(listener => listener.Invoke());
            
        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)} called twice in one frame!");
            }

            _frameId = ObservedUtility.frameId;
        #endif
        }
        
        public void SilentSend() => listeners.Invoke(listener => listener.Invoke());

        public void Unload() => listeners = null;
    }

#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T> : IUnload {
        internal List<Listener<T>> listeners = new List<Listener<T>>();

    #if UNITY_EDITOR
        private uint _frameId;
    #endif
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send([NotNullSystem] T data) {
            listeners.Invoke(listener => listener.Invoke(data));
            
        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T).Name}> called twice in one frame!");
            }

            _frameId = ObservedUtility.frameId;
        #endif
        }
        
        public void Send([NotNullSystem] params T[] data) {
            listeners.Invoke(listener => listener.Invoke(data));
            
        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T).Name}> called twice in one frame!");
            }

            _frameId = ObservedUtility.frameId;
        #endif
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void SendNull() {
            listeners.Invoke(listener => listener.InvokeNull());
            
        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T).Name}> called twice in one frame!");
            }

            _frameId = ObservedUtility.frameId;
        #endif
        }

        public void Unload() => listeners = null;
    }

#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2> : IUnload {
        internal List<Listener<T1, T2>> listeners = new List<Listener<T1, T2>>();

    #if UNITY_EDITOR
        private uint _frameId;
    #endif
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send([NotNullSystem] T1 data1, [NotNullSystem] T2 data2) {
            listeners.Invoke(listener => listener.Invoke(data1, data2));
            
        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T1).Name}, {typeof(T2).Name}> called twice in one frame!");
            }

            _frameId = ObservedUtility.frameId;
        #endif
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void SendNull([CanBeNull] T1 data1, [CanBeNull] T2 data2) {
            listeners.Invoke(listener => listener.InvokeNull(data1, data2));
            
        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T1).Name}, {typeof(T2).Name}> called twice in one frame!");
            }

            _frameId = ObservedUtility.frameId;
        #endif
        }

        public void Unload() => listeners = null;
    }
    
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2, T3> : IUnload {
        internal List<Listener<T1, T2, T3>> listeners = new List<Listener<T1, T2, T3>>();

    #if UNITY_EDITOR
        private uint _frameId;
    #endif
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send([NotNullSystem] T1 data1, [NotNullSystem] T2 data2, [NotNullSystem] T3 data3) {
            listeners.Invoke(listener => listener.Invoke(data1, data2, data3));
            
        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}> called twice in one frame!");
            }

            _frameId = ObservedUtility.frameId;
        #endif
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void SendNull([CanBeNull] T1 data1, [CanBeNull] T2 data2, [CanBeNull] T3 data3) {
            listeners.Invoke(listener => listener.InvokeNull(data1, data2, data3));
            
        #if PERFORMANCE_DEBUG
            if (_frameId == ObservedUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}> called twice in one frame!");
            }

            _frameId = ObservedUtility.frameId;
        #endif
        }

        public void Unload() => listeners = null;
    }
}