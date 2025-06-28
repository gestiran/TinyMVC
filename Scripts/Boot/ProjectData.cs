using System;
using System.Collections.Generic;
using TinyMVC.Dependencies;
using TinyMVC.Dependencies.Components;

namespace TinyMVC.Boot {
    public sealed class ProjectData {
        internal DependencyContainer tempContainer;
        internal DependencyContainer viewsContainer;
        
        internal readonly Dictionary<string, DependencyContainer> contexts;
        
        private readonly ProjectComponents _components;
        
        private const int _CAPACITY = 16;
        
    #if UNITY_EDITOR
        internal static event Action<string, DependencyContainer> onAdd;
        internal static event Action<string> onRemove;
    #endif
        
        internal ProjectData(ProjectComponents components) {
            contexts = new Dictionary<string, DependencyContainer>(_CAPACITY);
            _components = components;
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
        
        public bool GetReference<T>(out T dependency) {
            if (tempContainer != null) {
                foreach (IDependency current in tempContainer.dependencies.Values) {
                    if (current is not T tempValue) {
                        continue;
                    }
                    
                    dependency = tempValue;
                    return true;
                }
            }
            
            if (viewsContainer != null) {
                foreach (IDependency current in viewsContainer.dependencies.Values) {
                    if (current is not T tempValue) {
                        continue;
                    }
                    
                    dependency = tempValue;
                    return true;
                }
            }
            
            foreach (DependencyContainer container in contexts.Values) {
                foreach (IDependency current in container.dependencies.Values) {
                    if (current is not T tempValue) {
                        continue;
                    }
                    
                    dependency = tempValue;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public bool GetReference<T>(string contextKey, out T dependency) {
            if (tempContainer != null) {
                foreach (IDependency current in tempContainer.dependencies.Values) {
                    if (current is not T tempValue) {
                        continue;
                    }
                    
                    dependency = tempValue;
                    return true;
                }
            }
            
            if (viewsContainer != null) {
                foreach (IDependency current in viewsContainer.dependencies.Values) {
                    if (current is not T tempValue) {
                        continue;
                    }
                    
                    dependency = tempValue;
                    return true;
                }
            }
            
            if (contexts.TryGetValue(contextKey, out DependencyContainer container)) {
                foreach (IDependency current in container.dependencies.Values) {
                    if (current is not T tempValue) {
                        continue;
                    }
                    
                    dependency = tempValue;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        internal void Add(string contextKey, List<IDependency> dependencies) {
            if (contexts.TryGetValue(contextKey, out DependencyContainer container)) {
                foreach (IDependency dependency in dependencies) {
                    container.Update(dependency);
                }
            } else {
                container = new DependencyContainer(dependencies);
                _components.all.Add(contextKey, new Dictionary<Model, List<ModelComponent>>());
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
                _components.all.Add(contextKey, new Dictionary<Model, List<ModelComponent>>());
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
            _components.all.Remove(contextKey);
            contexts.Remove(contextKey);
            
        #if UNITY_EDITOR
            onRemove?.Invoke(contextKey);
        #endif
        }
    }
}