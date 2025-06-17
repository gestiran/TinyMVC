using System.Collections.Generic;
using TinyMVC.Dependencies.Components;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;
using UnityEngine;

namespace TinyMVC.Boot {
    public sealed class ProjectComponents {
        internal readonly Dictionary<string, Dictionary<Model, List<ModelComponent>>> all;
        private readonly Dictionary<string, IComponentListeners> _listeners;
        
        private const int _CAPACITY = 64;
        
        internal ProjectComponents() {
            all = new Dictionary<string, Dictionary<Model, List<ModelComponent>>>(_CAPACITY);
            _listeners = new Dictionary<string, IComponentListeners>(_CAPACITY);
        }
        
        public void AddOnAddListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener) where TModel : Model where TComponent : ModelComponent {
            string key = Key<TModel, TComponent>();
            
            if (_listeners.TryGetValue(key, out IComponentListeners value)) {
                if (value is ComponentListeners<TModel, TComponent> target) {
                    target.AddOnAddListener(listener);
                } else {
                    Debug.LogError($"ProjectComponents.AddOnAddListener<{typeof(TModel)}, {typeof(TComponent)}> - Invalid listeners!");
                }
            } else {
                ComponentListeners<TModel, TComponent> target = new ComponentListeners<TModel, TComponent>();
                target.AddOnAddListener(listener);
                _listeners.Add(key, target);
            }
        }
        
        public void AddOnAddListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener, UnloadPool unload) where TModel : Model where TComponent : ModelComponent {
            string key = Key<TModel, TComponent>();
            
            if (_listeners.TryGetValue(key, out IComponentListeners value)) {
                if (value is ComponentListeners<TModel, TComponent> target) {
                    target.AddOnAddListener(listener);
                    unload.Add(new UnloadAction(() => target.RemoveOnAddListener(listener)));
                } else {
                    Debug.LogError($"ProjectComponents.AddOnAddListener<{typeof(TModel)}, {typeof(TComponent)}> - Invalid listeners!");
                }
            } else {
                ComponentListeners<TModel, TComponent> target = new ComponentListeners<TModel, TComponent>();
                target.AddOnAddListener(listener);
                unload.Add(new UnloadAction(() => target.RemoveOnAddListener(listener)));
                _listeners.Add(key, target);
            }
        }
        
        public void RemoveOnAddListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener) where TModel : Model where TComponent : ModelComponent {
            string key = Key<TModel, TComponent>();
            
            if (_listeners.TryGetValue(key, out IComponentListeners value)) {
                if (value is ComponentListeners<TModel, TComponent> target) {
                    target.RemoveOnAddListener(listener);
                } else {
                    Debug.LogError($"ProjectComponents.RemoveOnAddListener<{typeof(TModel)}, {typeof(TComponent)}> - Invalid listeners!");
                }
            } else {
                ComponentListeners<TModel, TComponent> target = new ComponentListeners<TModel, TComponent>();
                target.RemoveOnAddListener(listener);
                _listeners.Add(key, target);
            }
        }
        
        public void AddOnRemoveListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener) where TModel : Model where TComponent : ModelComponent {
            string key = Key<TModel, TComponent>();
            
            if (_listeners.TryGetValue(key, out IComponentListeners value)) {
                if (value is ComponentListeners<TModel, TComponent> target) {
                    target.AddOnRemoveListener(listener);
                } else {
                    Debug.LogError($"ProjectComponents.AddOnRemoveListener<{typeof(TModel)}, {typeof(TComponent)}> - Invalid listeners!");
                }
            } else {
                ComponentListeners<TModel, TComponent> target = new ComponentListeners<TModel, TComponent>();
                target.AddOnRemoveListener(listener);
                _listeners.Add(key, target);
            }
        }
        
        public void AddOnRemoveListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener, UnloadPool unload) where TModel : Model where TComponent : ModelComponent {
            string key = Key<TModel, TComponent>();
            
            if (_listeners.TryGetValue(key, out IComponentListeners value)) {
                if (value is ComponentListeners<TModel, TComponent> target) {
                    target.AddOnRemoveListener(listener);
                    unload.Add(new UnloadAction(() => target.RemoveOnRemoveListener(listener)));
                } else {
                    Debug.LogError($"ProjectComponents.AddOnRemoveListener<{typeof(TModel)}, {typeof(TComponent)}> - Invalid listeners!");
                }
            } else {
                ComponentListeners<TModel, TComponent> target = new ComponentListeners<TModel, TComponent>();
                target.AddOnRemoveListener(listener);
                unload.Add(new UnloadAction(() => target.RemoveOnRemoveListener(listener)));
                _listeners.Add(key, target);
            }
        }
        
        public void RemoveOnRemoveListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener) where TModel : Model where TComponent : ModelComponent {
            string key = Key<TModel, TComponent>();
            
            if (_listeners.TryGetValue(key, out IComponentListeners value)) {
                if (value is ComponentListeners<TModel, TComponent> target) {
                    target.RemoveOnRemoveListener(listener);
                } else {
                    Debug.LogError($"ProjectComponents.RemoveOnRemoveListener<{typeof(TModel)}, {typeof(TComponent)}> - Invalid listeners!");
                }
            } else {
                ComponentListeners<TModel, TComponent> target = new ComponentListeners<TModel, TComponent>();
                target.RemoveOnRemoveListener(listener);
                _listeners.Add(key, target);
            }
        }
        
        internal void Add<TModel, TComponent>(TModel model, TComponent component) where TModel : Model where TComponent : ModelComponent {
            if (all.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(model, out List<ModelComponent> list)) {
                    list.Add(component);
                } else {
                    components.Add(model, new List<ModelComponent>() { component });
                }
                
                foreach (IComponentListeners listeners in _listeners.Values) {
                    listeners.TryInvokeAdd(model, component);
                }
            }
        }
        
        internal void Remove<TModel, TComponent>(TModel model, TComponent component) where TModel : Model where TComponent : ModelComponent {
            if (all.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(model, out List<ModelComponent> list) && list.Remove(component)) {
                    foreach (IComponentListeners listeners in _listeners.Values) {
                        listeners.TryInvokeRemove(model, component);
                    }
                }
            }
        }
        
        public IEnumerable<ComponentLink<T>> ForEach<T>() {
            List<ComponentLink<T>> temp = new List<ComponentLink<T>>();
            
            foreach (Dictionary<Model, List<ModelComponent>> components in all.Values) {
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
        
        public IEnumerable<ComponentLink<T>> ForEach<T>(string contextKey) {
            List<ComponentLink<T>> temp = new List<ComponentLink<T>>();
            
            if (all.TryGetValue(contextKey, out Dictionary<Model, List<ModelComponent>> components)) {
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
        
        private string Key<TModel, TComponent>() where TModel : Model where TComponent : ModelComponent => $"{typeof(TModel).Name}#{typeof(TComponent).Name}";
    }
}