using System;
using System.Collections.Generic;
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
        private readonly ObservedDictionary<string, ModelComponent> _components;
        
        private readonly int _id;
        
        private static int _globalId;
        
        private const int _CAPACITY = 16;
        
        protected Model() {
            _components = new ObservedDictionary<string, ModelComponent>(_CAPACITY);
            _id = _globalId++;
        }
        
        public void AddComponent<T>(T component) where T : ModelComponent => _components.Add(typeof(T).FullName, component);
        
        public void RemoveComponent<T>() where T : ModelComponent => _components.RemoveByKey(typeof(T).FullName);
        
        public void RemoveComponents(List<ModelComponent> components) => _components.RemoveRange(components);
        
        public bool IsHaveComponent<T>() where T : ModelComponent => _components.ContainsKey(typeof(T).FullName);
        
        public bool TryGetComponent<T>(out T component) where T : ModelComponent {
            if (_components.TryGetValue(typeof(T).FullName, out ModelComponent componentValue)) {
                component = (T)componentValue;
                
                return true;
            }
            
            component = default;
            
            return false;
        }
        
        public IEnumerable<T> ForEach<T>() {
            foreach (ModelComponent component in _components.ForEachValues()) {
                if (component is T target) {
                    yield return target;
                }
            }
        }
        
        public IEnumerable<(T1, T2)> ForEach<T1, T2>() {
            IEnumerable<ModelComponent> values = _components.ForEachValues();
            
            foreach (ModelComponent component in values) {
                if (component is not T1 target1) {
                    continue;
                }
                
                if (component is not T2 target2) {
                    continue;
                }
                
                yield return (target1, target2);
            }
        }
        
        public IEnumerable<(T1, T2, T3)> ForEach<T1, T2, T3>() {
            IEnumerable<ModelComponent> values = _components.ForEachValues();
            
            foreach (ModelComponent component in values) {
                if (component is not T1 target1) {
                    continue;
                }
                
                if (component is not T2 target2) {
                    continue;
                }
                
                if (component is not T3 target3) {
                    continue;
                }
                
                yield return (target1, target2, target3);
            }
        }
        
        public IEnumerable<(T1, T2, T3, T4)> ForEach<T1, T2, T3, T4>() {
            IEnumerable<ModelComponent> values = _components.ForEachValues();
            
            foreach (ModelComponent component in values) {
                if (component is not T1 target1) {
                    continue;
                }
                
                if (component is not T2 target2) {
                    continue;
                }
                
                if (component is not T3 target3) {
                    continue;
                }
                
                if (component is not T4 target4) {
                    continue;
                }
                
                yield return (target1, target2, target3, target4);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener listener) => _components.AddOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener listener, UnloadPool unload) => _components.AddOnAddListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener<ModelComponent> listener) => _components.AddOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener<ModelComponent> listener, UnloadPool unload) => _components.AddOnAddListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnAddListener(ActionListener listener) => _components.RemoveOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnAddListener(ActionListener<ModelComponent> listener) => _components.RemoveOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener listener) => _components.AddOnRemoveListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener listener, UnloadPool unload) => _components.AddOnRemoveListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener<ModelComponent> listener) => _components.AddOnRemoveListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener<ModelComponent> listener, UnloadPool unload) => _components.AddOnRemoveListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnRemoveListener(ActionListener listener) => _components.RemoveOnRemoveListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnRemoveListener(ActionListener<ModelComponent> listener) => _components.RemoveOnRemoveListener(listener);
        
    #if UNITY_EDITOR
        
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset_Editor() => _globalId = 0;
        
    #endif
        
        
        public bool Equals(Model other) => other != null && other._id == _id;
        
        public override int GetHashCode() => _id;
    }
}