using System.Collections.Generic;
using TinyMVC.Dependencies.Components;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;

namespace TinyMVC.Boot {
    public sealed class ProjectComponents {
        internal readonly Dictionary<string, Dictionary<Model, List<ModelComponent>>> all;
        
        private const int _CAPACITY = 64;
        
        internal ProjectComponents() {
            all = new Dictionary<string, Dictionary<Model, List<ModelComponent>>>(_CAPACITY);
        }
        
        public void AddOnAddListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener) where TModel : Model where TComponent : ModelComponent {
            ModelComponents<TModel, TComponent>.AddOnAddListener(listener);
        }
        
        public void AddOnAddListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener, UnloadPool unload) where TModel : Model where TComponent : ModelComponent {
            ModelComponents<TModel, TComponent>.AddOnAddListener(listener);
            unload.Add(new UnloadAction(() => ModelComponents<TModel, TComponent>.RemoveOnAddListener(listener)));
        }
        
        public void RemoveOnAddListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener) where TModel : Model where TComponent : ModelComponent {
            ModelComponents<TModel, TComponent>.RemoveOnAddListener(listener);
        }
        
        public void AddOnRemoveListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener) where TModel : Model where TComponent : ModelComponent {
            ModelComponents<TModel, TComponent>.AddOnRemoveListener(listener);
        }
        
        public void AddOnRemoveListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener, UnloadPool unload) where TModel : Model where TComponent : ModelComponent {
            ModelComponents<TModel, TComponent>.AddOnRemoveListener(listener);
            unload.Add(new UnloadAction(() => ModelComponents<TModel, TComponent>.RemoveOnRemoveListener(listener)));
        }
        
        public void RemoveOnRemoveListener<TModel, TComponent>(ActionListener<TModel, TComponent> listener) where TModel : Model where TComponent : ModelComponent {
            ModelComponents<TModel, TComponent>.RemoveOnRemoveListener(listener);
        }
        
        internal void Add<TModel, TComponent>(TModel model, TComponent component) where TModel : Model where TComponent : ModelComponent {
            if (all.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(model, out List<ModelComponent> list)) {
                    list.Add(component);
                } else {
                    components.Add(model, new List<ModelComponent>() { component });
                }
                
                ModelComponents<TModel, TComponent>.InvokeAdd(model, component);
            }
        }
        
        internal void Remove<TModel, TComponent>(TModel model, TComponent component) where TModel : Model where TComponent : ModelComponent {
            if (all.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(model, out List<ModelComponent> list) && list.Remove(component)) {
                    ModelComponents<TModel, TComponent>.InvokeRemove(model, component);
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
    }
}