using System;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies.Components {
    /// <summary> Marker of an object marking it as a possible dependency </summary>
    #if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox, ShowInInspector]
    #endif
    public abstract class ModelComponent : IEquatable<ModelComponent> {
        public int id { get; }
        
        protected ModelComponent() => id = GetId();
        
        private static int _globalId;
        
        public static int GetId() => _globalId++;
        
        #if UNITY_EDITOR
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset_Editor() => _globalId = 0;
        
        #endif
        
        public bool Equals(ModelComponent other) => other != null && other.id == id;
        
        public override int GetHashCode() => id;
    }
}