﻿using System.Collections.Generic;
using JetBrains.Annotations;
using TinyMVC.Loop;

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
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].Invoke();
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)} called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }

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
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].Invoke(data);
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T).Name}> called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }
        
        public void Send([NotNullSystem] params T[] data) {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].Invoke(data);
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T).Name}> called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void SendNull() {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].InvokeNull();
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T).Name}> called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
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
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].Invoke(data1, data2);
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T1).Name}, {typeof(T2).Name}> called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void SendNull([CanBeNull] T1 data1, [CanBeNull] T2 data2) {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].InvokeNull(data1, data2);
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T1).Name}, {typeof(T2).Name}> called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
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
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].Invoke(data1, data2, data3);
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}> called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void SendNull([CanBeNull] T1 data1, [CanBeNull] T2 data2, [CanBeNull] T3 data3) {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].InvokeNull(data1, data2, data3);
            }
            
        #if UNITY_EDITOR
            if (_frameId == ObservedTestUtility.frameId) {
                UnityEngine.Debug.LogWarning($"{nameof(InputListener)}<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}> called twice in one frame!");
            }

            _frameId = ObservedTestUtility.frameId;
        #endif
        }

        public void Unload() => listeners = null;
    }
}