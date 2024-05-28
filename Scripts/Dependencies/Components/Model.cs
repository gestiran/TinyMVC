using System;
using System.Collections.Generic;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies.Components {
    /// <summary> Marker of an object marking it as a possible dependency </summary>
#if ODIN_INSPECTOR && UNITY_EDITOR
    [HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public abstract class Model : IDependency, IEquatable<Model> {
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, HideLabel]
    #endif
        private readonly ObservedDictionary<Type, ModelComponent> _components;
        private readonly int _id;

        private static int _globalId;
        
        private const int _CAPACITY = 16;

        protected Model() {
            _components = new ObservedDictionary<Type, ModelComponent>(_CAPACITY);
            _id = _globalId++;
        }

        public void AddComponent<T>(T component) where T : ModelComponent => _components.Add(typeof(T), component);

        public void RemoveComponent<T>() where T : ModelComponent => _components.RemoveByKey(typeof(T));

        public void RemoveComponents(List<ModelComponent> components) => _components.RemoveRange(components);

        public bool IsHaveComponent<T>() where T : ModelComponent => _components.ContainsKey(typeof(T));

        public bool TryGetComponent<T>(out T component) where T : ModelComponent {
            if (_components.TryGetValue(typeof(T), out ModelComponent componentValue)) {
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

        public void AddOnAddListener(Action listener) => _components.AddOnAddListener(listener);

        public void AddOnAddListener(Action listener, UnloadPool unload) => _components.AddOnAddListener(listener, unload);

        public void AddOnAddListener(Action<ModelComponent> listener) => _components.AddOnAddListener(listener);

        public void AddOnAddListener(Action<ModelComponent> listener, UnloadPool unload) => _components.AddOnAddListener(listener, unload);

        public void RemoveOnAddListener(Action listener) => _components.RemoveOnAddListener(listener);

        public void RemoveOnAddListener(Action<ModelComponent> listener) => _components.RemoveOnAddListener(listener);
        
        public void AddOnRemoveListener(Action listener) => _components.AddOnRemoveListener(listener);

        public void AddOnRemoveListener(Action listener, UnloadPool unload) => _components.AddOnRemoveListener(listener, unload);

        public void AddOnRemoveListener(Action<ModelComponent> listener) => _components.AddOnRemoveListener(listener);

        public void AddOnRemoveListener(Action<ModelComponent> listener, UnloadPool unload) => _components.AddOnRemoveListener(listener, unload);

        public void RemoveOnRemoveListener(Action listener) => _components.RemoveOnRemoveListener(listener);

        public void RemoveOnRemoveListener(Action<ModelComponent> listener) => _components.RemoveOnRemoveListener(listener);

    #if UNITY_EDITOR
        
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset_Editor() => _globalId = 0;

    #endif
        
        
        public bool Equals(Model other) => other != null && other._id == _id;

        public override int GetHashCode() => _id;
    }
}