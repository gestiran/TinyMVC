// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyMVC.Dependencies.Components;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;
using UnityEngine;

namespace TinyMVC.Boot {
    public sealed class ProjectComponents {
        internal readonly Dictionary<string, Dictionary<int, Model>> all;
        private readonly Dictionary<string, IComponentListeners> _listeners;
        
        private const int _CAPACITY = 64;
        
        internal ProjectComponents() {
            all = new Dictionary<string, Dictionary<int, Model>>(_CAPACITY);
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
            if (all.TryGetValue(ProjectContext.activeContext.key, out Dictionary<int, Model> data)) {
                data.TryAdd(model.GetHashCode(), model);
                
                foreach (IComponentListeners listeners in _listeners.Values) {
                    listeners.TryInvokeAdd(model, component);
                }
            }
        }
        
        internal void Remove<TModel, TComponent>(TModel model, TComponent component) where TModel : Model where TComponent : ModelComponent {
            if (all.TryGetValue(ProjectContext.activeContext.key, out Dictionary<int, Model> data)) {
                int key = model.GetHashCode();
                
                if (data.TryGetValue(key, out Model target)) {
                    if (target.components.count == 0) {
                        data.Remove(key);
                    }
                    
                    foreach (IComponentListeners listeners in _listeners.Values) {
                        listeners.TryInvokeRemove(model, component);
                    }
                }
            }
        }
        
        public IEnumerable<ComponentLink<T>> ForEach<T>() {
            List<ComponentLink<T>> temp = new List<ComponentLink<T>>();
            
            foreach (Dictionary<int, Model> data in all.Values) {
                foreach (Model model in data.Values) {
                    foreach (ModelComponent component in model.components.root.Values) {
                        if (component is T other) {
                            temp.Add(new ComponentLink<T>(model, other));
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
            
            if (all.TryGetValue(contextKey, out Dictionary<int, Model> data)) {
                foreach (Model model in data.Values) {
                    foreach (ModelComponent component in model.components.root.Values) {
                        if (component is T other) {
                            temp.Add(new ComponentLink<T>(model, other));
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