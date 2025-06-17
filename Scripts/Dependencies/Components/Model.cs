using System;
using System.Collections.Generic;
using TinyMVC.Boot;
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
        internal readonly ObservedDictionary<string, ModelComponent> componentsList;
        
        private readonly int _id;
        
        private static int _globalId;
        
        private const int _CAPACITY = 16;
        
        protected Model() {
            componentsList = new ObservedDictionary<string, ModelComponent>(_CAPACITY);
            _id = _globalId++;
        }
        
        public void AddComponent<T>(T component) where T : ModelComponent => AddComponentInternal(component);
        
        internal protected virtual void AddComponentInternal<T>(T component) where T : ModelComponent {
            ProjectContext.components.Add(this, component);
            componentsList.Add(typeof(T).FullName, component);
        }
        
        public void RemoveComponent<T>() where T : ModelComponent => RemoveComponentInternal(typeof(T).FullName);
        
        public void RemoveComponent<T>(T component) where T : ModelComponent => RemoveComponentInternal(component.GetType().FullName);
        
        public void RemoveComponents<T>(T components) where T : IEnumerable<ModelComponent> {
            foreach (ModelComponent component in components) {
                RemoveComponentInternal(component.GetType().FullName);
            }
        }
        
        internal protected virtual void RemoveComponentInternal(string key) {
            if (componentsList.TryGetValue(key, out ModelComponent component)) {
                ProjectContext.components.Remove(this, component);
                componentsList.RemoveByKey(key);
            }
        }
        
        public bool IsHaveComponent<T>() where T : ModelComponent => componentsList.ContainsKey(typeof(T).FullName);
        
        public bool IsHaveComponent<T>(T component) where T : ModelComponent => componentsList.ContainsKey(component.GetType().FullName);
        
        public bool TryGetComponent<T>(out T component) where T : ModelComponent {
            if (componentsList.TryGetValue(typeof(T).FullName, out ModelComponent componentValue)) {
                component = (T)componentValue;
                return true;
            }
            
            component = null;
            return false;
        }
        
        public IEnumerable<T> ForEach<T>() {
            foreach (ModelComponent component in componentsList.ForEachValues()) {
                if (component is T target) {
                    yield return target;
                }
            }
        }
        
        public IEnumerable<(T1, T2)> ForEach<T1, T2>() {
            IEnumerable<ModelComponent> values = componentsList.ForEachValues();
            
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
            IEnumerable<ModelComponent> values = componentsList.ForEachValues();
            
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
            IEnumerable<ModelComponent> values = componentsList.ForEachValues();
            
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
        public void AddOnAddListener(ActionListener listener) => componentsList.AddOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener listener, UnloadPool unload) => componentsList.AddOnAddListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener<ModelComponent> listener) => componentsList.AddOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnAddListener(ActionListener<ModelComponent> listener, UnloadPool unload) => componentsList.AddOnAddListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnAddListener(ActionListener listener) => componentsList.RemoveOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnAddListener(ActionListener<ModelComponent> listener) => componentsList.RemoveOnAddListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener listener) => componentsList.AddOnRemoveListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener listener, UnloadPool unload) => componentsList.AddOnRemoveListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener<ModelComponent> listener) => componentsList.AddOnRemoveListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddOnRemoveListener(ActionListener<ModelComponent> listener, UnloadPool unload) => componentsList.AddOnRemoveListener(listener, unload);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnRemoveListener(ActionListener listener) => componentsList.RemoveOnRemoveListener(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveOnRemoveListener(ActionListener<ModelComponent> listener) => componentsList.RemoveOnRemoveListener(listener);
        
    #if UNITY_EDITOR
        
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset_Editor() => _globalId = 0;
        
    #endif
        
        
        public bool Equals(Model other) => other != null && other._id == _id;
        
        public override int GetHashCode() => _id;
    }
}