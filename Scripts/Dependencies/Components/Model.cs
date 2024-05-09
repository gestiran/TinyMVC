using System;

using Sirenix.OdinInspector;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;
#if ODIN_INSPECTOR && UNITY_EDITOR
#endif

namespace TinyMVC.Dependencies.Components {
    /// <summary> Marker of an object marking it as a possible dependency </summary>
#if ODIN_INSPECTOR && UNITY_EDITOR
    [HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public abstract class Model : IDependency {
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, HideLabel]
    #endif
        private readonly ObservedList<IModelComponent> _components;

        private const int _CAPACITY = 16;
        
        protected Model() => _components = new ObservedList<IModelComponent>(_CAPACITY);

        public void AddComponent<T>(T component) where T : IModelComponent => _components.Add(component);

        public void RemoveComponent<T>(T component) where T : IModelComponent => _components.Remove(component);

        public void RemoveComponent<T>() where T : IModelComponent {
            for (int componentId = 0; componentId < _components.count; componentId++) {
                if (_components[componentId] is T) {
                    _components.RemoveAt(componentId);
                    return;
                }
            }
        }

        public bool IsHaveComponent<T>() where T : IModelComponent {
            for (int componentId = 0; componentId < _components.count; componentId++) {
                if (_components[componentId] is T) {
                    return true;
                }
            }

            return false;
        }
        
        public bool TryGetComponent<T>(out T component) where T : IModelComponent {
            for (int componentId = 0; componentId < _components.count; componentId++) {
                if (_components[componentId] is T target) {
                    component = target;
                    return true;
                }
            }

            component = default;
            return false;
        }

        public void AddOnAddListener(Action listener) => _components.AddOnAddListener(listener);

        public void AddOnAddListener(Action listener, UnloadPool unload) => _components.AddOnAddListener(listener, unload);

        public void AddOnAddListener(Action<IModelComponent> listener) => _components.AddOnAddListener(listener);

        public void AddOnAddListener(Action<IModelComponent> listener, UnloadPool unload) => _components.AddOnAddListener(listener, unload);

        public void RemoveOnAddListener(Action listener) => _components.RemoveOnAddListener(listener);

        public void RemoveOnAddListener(Action<IModelComponent> listener) => _components.RemoveOnAddListener(listener);
        
        public void AddOnRemoveListener(Action listener) => _components.AddOnRemoveListener(listener);

        public void AddOnRemoveListener(Action listener, UnloadPool unload) => _components.AddOnRemoveListener(listener, unload);

        public void AddOnRemoveListener(Action<IModelComponent> listener) => _components.AddOnRemoveListener(listener);

        public void AddOnRemoveListener(Action<IModelComponent> listener, UnloadPool unload) => _components.AddOnRemoveListener(listener, unload);

        public void RemoveOnRemoveListener(Action listener) => _components.RemoveOnRemoveListener(listener);

        public void RemoveOnRemoveListener(Action<IModelComponent> listener) => _components.RemoveOnRemoveListener(listener);
        
        public void AddOnClearListener(Action listener) => _components.AddOnClearListener(listener);

        public void AddOnClearListener(Action listener, UnloadPool unload) => _components.AddOnClearListener(listener, unload);

        public void RemoveOnClearListener(Action listener) => _components.RemoveOnClearListener(listener);
        
        public void Clear() => _components.Clear();
    }
}