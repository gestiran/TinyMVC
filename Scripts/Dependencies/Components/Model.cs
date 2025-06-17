using System;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies.Components {
#if UNITY_EDITOR
    [HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public abstract class Model : IDependency, IEquatable<Model> {
    #if UNITY_EDITOR
        [ShowInInspector, HideLabel]
    #endif
        internal readonly ObservedDictionary<string, ModelComponent> components;
        
        private readonly int _id;
        
        private static int _globalId;
        
        private const int _CAPACITY = 16;
        
        protected Model() {
            components = new ObservedDictionary<string, ModelComponent>(_CAPACITY);
            _id = _globalId++;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener listener) => components.AddOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener listener, UnloadPool unload) => components.AddOnAddListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener<ModelComponent> listener) => components.AddOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener<ModelComponent> listener, UnloadPool unload) => components.AddOnAddListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnAddListener(ActionListener listener) => components.RemoveOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnAddListener(ActionListener<ModelComponent> listener) => components.RemoveOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener listener) => components.AddOnRemoveListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener listener, UnloadPool unload) => components.AddOnRemoveListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener<ModelComponent> listener) => components.AddOnRemoveListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener<ModelComponent> listener, UnloadPool unload) => components.AddOnRemoveListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnRemoveListener(ActionListener listener) => components.RemoveOnRemoveListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnRemoveListener(ActionListener<ModelComponent> listener) => components.RemoveOnRemoveListener(listener);
        
    #if UNITY_EDITOR
        
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset_Editor() => _globalId = 0;
        
    #endif
        
        
        public bool Equals(Model other) => other != null && other._id == _id;
        
        public override int GetHashCode() => _id;
    }
}