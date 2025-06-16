using System;
using System.Collections.Generic;
using TinyMVC.Dependencies;
using TinyMVC.Dependencies.Components;

namespace TinyMVC.Boot {
    public sealed class ProjectData {
        internal DependencyContainer tempContainer;
        internal DependencyContainer viewsContainer;
        
        internal readonly Dictionary<string, DependencyContainer> contexts;
        internal readonly Dictionary<string, Dictionary<Model, List<ModelComponent>>> contextComponents;
        
        private const int _CAPACITY = 16;
        
    #if UNITY_EDITOR
        internal static event Action<string, DependencyContainer> onAdd;
        internal static event Action<string> onRemove;
    #endif
        
        internal ProjectData() {
            contexts = new Dictionary<string, DependencyContainer>(_CAPACITY);
            contextComponents = new Dictionary<string, Dictionary<Model, List<ModelComponent>>>(_CAPACITY);
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
        
        public IEnumerable<ComponentLink<T>> ForEachComponents<T>() where T : ModelComponent {
            List<ComponentLink<T>> temp = new List<ComponentLink<T>>();
            
            foreach (Dictionary<Model, List<ModelComponent>> components in contextComponents.Values) {
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
        
        public IEnumerable<ComponentLink<T>> ForEachComponents<T>(string contextKey) where T : ModelComponent {
            List<ComponentLink<T>> temp = new List<ComponentLink<T>>();
            
            if (contextComponents.TryGetValue(contextKey, out Dictionary<Model, List<ModelComponent>> components)) {
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
                contextComponents.Add(contextKey, new Dictionary<Model, List<ModelComponent>>());
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
                contextComponents.Add(contextKey, new Dictionary<Model, List<ModelComponent>>());
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
            contextComponents.Remove(contextKey);
            contexts.Remove(contextKey);
            
        #if UNITY_EDITOR
            onRemove?.Invoke(contextKey);
        #endif
        }
        
        internal void AddComponent(Model model, ModelComponent component) {
            if (contextComponents.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(model, out List<ModelComponent> list)) {
                    list.Add(component);
                } else {
                    components.Add(model, new List<ModelComponent>() { component });
                }
            }
        }
        
        internal void RemoveComponent(Model model, ModelComponent component) {
            if (contextComponents.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(model, out List<ModelComponent> list)) {
                    list.Remove(component);
                }
            }
        }
    }
}