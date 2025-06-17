using System;
using System.Collections.Generic;
using TinyMVC.Dependencies;
using TinyMVC.Dependencies.Components;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;
using TinyMVC.ReactiveFields.Extensions;

namespace TinyMVC.Boot {
    public sealed class ProjectData {
        internal DependencyContainer tempContainer;
        internal DependencyContainer viewsContainer;
        
        internal readonly Dictionary<string, DependencyContainer> contexts;
        
        private readonly Dictionary<string, Dictionary<Model, List<ModelComponent>>> _contextComponents;
        private readonly List<ActionListener<Model, ModelComponent>> _addComponentListeners;
        private readonly List<ActionListener<Model, ModelComponent>> _removeComponentListeners;
        
        private const int _CAPACITY = 16;
        
    #if UNITY_EDITOR
        internal static event Action<string, DependencyContainer> onAdd;
        internal static event Action<string> onRemove;
    #endif
        
        internal ProjectData() {
            contexts = new Dictionary<string, DependencyContainer>(_CAPACITY);
            _contextComponents = new Dictionary<string, Dictionary<Model, List<ModelComponent>>>(_CAPACITY);
            _addComponentListeners = new List<ActionListener<Model, ModelComponent>>(_CAPACITY);
            _removeComponentListeners = new List<ActionListener<Model, ModelComponent>>(_CAPACITY);
        }
        
        public bool TryGetDependency<T>(out T dependency) where T : IDependency {
            Type type = typeof(T);
            
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = (T)tempValue;
                return true;
            }
            
            if (viewsContainer != null && viewsContainer.dependencies.TryGetValue(type, out tempValue)) {
                dependency = (T)tempValue;
                return true;
            }
            
            foreach (DependencyContainer container in contexts.Values) {
                if (container.dependencies.TryGetValue(type, out IDependency value)) {
                    dependency = (T)value;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public bool TryGetDependency<T>(string contextKey, out T dependency) where T : IDependency {
            Type type = typeof(T);
            
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = (T)tempValue;
                return true;
            }
            
            if (viewsContainer != null && viewsContainer.dependencies.TryGetValue(type, out tempValue)) {
                dependency = (T)tempValue;
                return true;
            }
            
            if (contexts.TryGetValue(contextKey, out DependencyContainer container)) {
                if (container.dependencies.TryGetValue(type, out IDependency value)) {
                    dependency = (T)value;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public bool TryGetDependency(string contextKey, Type type, out IDependency dependency) {
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = tempValue;
                return true;
            }
            
            if (viewsContainer != null && viewsContainer.dependencies.TryGetValue(type, out tempValue)) {
                dependency = tempValue;
                return true;
            }
            
            if (contexts.TryGetValue(contextKey, out DependencyContainer container)) {
                if (container.dependencies.TryGetValue(type, out IDependency value)) {
                    dependency = value;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public bool TryGetDependency(Type type, out IDependency dependency) {
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = tempValue;
                return true;
            }
            
            if (viewsContainer != null && viewsContainer.dependencies.TryGetValue(type, out tempValue)) {
                dependency = tempValue;
                return true;
            }
            
            foreach (DependencyContainer container in contexts.Values) {
                if (container.dependencies.TryGetValue(type, out IDependency value)) {
                    dependency = value;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public void AddOnAddComponentListener(ActionListener<Model, ModelComponent> listener) {
            _addComponentListeners.Add(listener);
        }
        
        public void AddOnAddComponentListener(ActionListener<Model, ModelComponent> listener, UnloadPool unload) {
            _addComponentListeners.Add(listener);
            unload.Add(new UnloadAction(() => _addComponentListeners.Remove(listener)));
        }
        
        public void RemoveOnAddComponentListener(ActionListener<Model, ModelComponent> listener) {
            _removeComponentListeners.Add(listener);
        }
        
        public void RemoveOnAddComponentListener(ActionListener<Model, ModelComponent> listener, UnloadPool unload) {
            _removeComponentListeners.Add(listener);
            unload.Add(new UnloadAction(() => _removeComponentListeners.Remove(listener)));
        }
        
        public void AddOnRemoveComponentListener(ActionListener<Model, ModelComponent> listener) {
            _addComponentListeners.Remove(listener);
        }
        
        public void RemoveOnRemoveComponentListener(ActionListener<Model, ModelComponent> listener) {
            _removeComponentListeners.Remove(listener);
        }
        
        public bool Get<T>(out T dependency) where T : IDependency {
            Type type = typeof(T);
            
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = (T)tempValue;
                return true;
            }
            
            if (viewsContainer != null && viewsContainer.dependencies.TryGetValue(type, out tempValue)) {
                dependency = (T)tempValue;
                return true;
            }
            
            foreach (DependencyContainer container in contexts.Values) {
                if (container.dependencies.TryGetValue(type, out IDependency value)) {
                    dependency = (T)value;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public bool Get<T>(string contextKey, out T dependency) where T : IDependency {
            Type type = typeof(T);
            
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = (T)tempValue;
                return true;
            }
            
            if (viewsContainer != null && viewsContainer.dependencies.TryGetValue(type, out tempValue)) {
                dependency = (T)tempValue;
                return true;
            }
            
            if (contexts.TryGetValue(contextKey, out DependencyContainer container) && container.dependencies.TryGetValue(type, out IDependency value)) {
                dependency = (T)value;
                return true;
            }
            
            dependency = default;
            return false;
        }
        
        public IEnumerable<ComponentLink<T>> ForEachComponents<T>() {
            List<ComponentLink<T>> temp = new List<ComponentLink<T>>();
            
            foreach (Dictionary<Model, List<ModelComponent>> components in _contextComponents.Values) {
                foreach (KeyValuePair<Model, List<ModelComponent>> pair in components) {
                    foreach (ModelComponent component in pair.Value) {
                        if (component is T other) {
                            temp.Add(new ComponentLink<T>(pair.Key, other));
                        }
                    }
                }
            }
            
            foreach (ComponentLink<T> result in temp) {
                yield return result;
            }
        }
        
        public IEnumerable<ComponentLink<T>> ForEachComponents<T>(string contextKey) {
            List<ComponentLink<T>> temp = new List<ComponentLink<T>>();
            
            if (_contextComponents.TryGetValue(contextKey, out Dictionary<Model, List<ModelComponent>> components)) {
                foreach (KeyValuePair<Model, List<ModelComponent>> pair in components) {
                    foreach (ModelComponent component in pair.Value) {
                        if (component is T other) {
                            temp.Add(new ComponentLink<T>(pair.Key, other));
                        }
                    }
                }
            }
            
            foreach (ComponentLink<T> result in temp) {
                yield return result;
            }
        }
        
        internal void Add(string contextKey, List<IDependency> dependencies) {
            if (contexts.TryGetValue(contextKey, out DependencyContainer container)) {
                foreach (IDependency dependency in dependencies) {
                    container.Update(dependency);
                }
            } else {
                container = new DependencyContainer(dependencies);
                _contextComponents.Add(contextKey, new Dictionary<Model, List<ModelComponent>>());
                contexts.Add(contextKey, container);
                
            #if UNITY_EDITOR
                onAdd?.Invoke(contextKey, container);
            #endif
            }
        }
        
        internal void Add(string contextKey, IDependency dependency) {
            if (contexts.TryGetValue(contextKey, out DependencyContainer container)) {
                container.Update(dependency);
            } else {
                container = new DependencyContainer(dependency);
                _contextComponents.Add(contextKey, new Dictionary<Model, List<ModelComponent>>());
                contexts.Add(contextKey, container);
                
            #if UNITY_EDITOR
                onAdd?.Invoke(contextKey, container);
            #endif
            }
        }
        
        internal void Remove(string contextKey) {
            if (contexts.TryGetValue(contextKey, out DependencyContainer container) == false) {
                return;
            }
            
            container.Dispose();
            _contextComponents.Remove(contextKey);
            contexts.Remove(contextKey);
            
        #if UNITY_EDITOR
            onRemove?.Invoke(contextKey);
        #endif
        }
        
        internal void AddComponent(Model model, ModelComponent component) {
            if (_contextComponents.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(model, out List<ModelComponent> list)) {
                    list.Add(component);
                } else {
                    components.Add(model, new List<ModelComponent>() { component });
                }
                
                _addComponentListeners.Invoke(model, component);
            }
        }
        
        internal void RemoveComponent(Model model, ModelComponent component) {
            if (_contextComponents.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(model, out List<ModelComponent> list) && list.Remove(component)) {
                    _removeComponentListeners.Invoke(model, component);
                }
            }
        }
    }
}