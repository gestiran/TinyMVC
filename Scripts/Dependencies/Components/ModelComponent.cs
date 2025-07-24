using System;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies.Components {
#if ODIN_INSPECTOR
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