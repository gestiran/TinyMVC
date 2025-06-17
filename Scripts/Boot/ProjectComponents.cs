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
        
    #region Models
        
        public void AddOnAddListener<T>(ActionListener<Model, T> listener) where T : ModelComponent {
            ModelComponents<T>.AddOnAddListener(listener);
        }
        
        public void AddOnAddListener<T>(ActionListener<Model, T> listener, UnloadPool unload) where T : ModelComponent {
            ModelComponents<T>.AddOnAddListener(listener);
            unload.Add(new UnloadAction(() => ModelComponents<T>.RemoveOnAddListener(listener)));
        }
        
        public void RemoveOnAddListener<T>(ActionListener<Model, T> listener) where T : ModelComponent {
            ModelComponents<T>.RemoveOnAddListener(listener);
        }
        
        public void AddOnRemoveListener<T>(ActionListener<Model, T> listener) where T : ModelComponent {
            ModelComponents<T>.AddOnRemoveListener(listener);
        }
        
        public void AddOnRemoveListener<T>(ActionListener<Model, T> listener, UnloadPool unload) where T : ModelComponent {
            ModelComponents<T>.AddOnRemoveListener(listener);
            unload.Add(new UnloadAction(() => ModelComponents<T>.RemoveOnRemoveListener(listener)));
        }
        
        public void RemoveOnRemoveListener<T>(ActionListener<Model, T> listener) where T : ModelComponent {
            ModelComponents<T>.RemoveOnRemoveListener(listener);
        }
        
    #endregion
        
    #region Actors
        
        public void AddOnAddListener<T>(ActionListener<Actor, T> listener) where T : ModelComponent {
            ActorComponents<T>.AddOnAddListener(listener);
        }
        
        public void AddOnAddListener<T>(ActionListener<Actor, T> listener, UnloadPool unload) where T : ModelComponent {
            ActorComponents<T>.AddOnAddListener(listener);
            unload.Add(new UnloadAction(() => ActorComponents<T>.RemoveOnAddListener(listener)));
        }
        
        public void RemoveOnAddListener<T>(ActionListener<Actor, T> listener) where T : ModelComponent {
            ActorComponents<T>.RemoveOnAddListener(listener);
        }
        
        public void AddOnRemoveListener<T>(ActionListener<Actor, T> listener) where T : ModelComponent {
            ActorComponents<T>.AddOnRemoveListener(listener);
        }
        
        public void AddOnRemoveListener<T>(ActionListener<Actor, T> listener, UnloadPool unload) where T : ModelComponent {
            ActorComponents<T>.AddOnRemoveListener(listener);
            unload.Add(new UnloadAction(() => ActorComponents<T>.RemoveOnRemoveListener(listener)));
        }
        
        public void RemoveOnRemoveListener<T>(ActionListener<Actor, T> listener) where T : ModelComponent {
            ActorComponents<T>.RemoveOnRemoveListener(listener);
        }
        
    #endregion
        
        internal void Add<T>(Model model, T component) where T : ModelComponent {
            if (all.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(model, out List<ModelComponent> list)) {
                    list.Add(component);
                } else {
                    components.Add(model, new List<ModelComponent>() { component });
                }
                
                ModelComponents<T>.InvokeAdd(model, component);
            }
        }
        
        internal void Add<T>(Actor actor, T component) where T : ModelComponent {
            if (all.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(actor, out List<ModelComponent> list)) {
                    list.Add(component);
                } else {
                    components.Add(actor, new List<ModelComponent>() { component });
                }
                
                ActorComponents<T>.InvokeAdd(actor, component);
            }
        }
        
        internal void Remove<T>(Model model, T component) where T : ModelComponent {
            if (all.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(model, out List<ModelComponent> list) && list.Remove(component)) {
                    ModelComponents<T>.InvokeRemove(model, component);
                }
            }
        }
        
        internal void Remove<T>(Actor actor, T component) where T : ModelComponent {
            if (all.TryGetValue(ProjectContext.activeContext.key, out Dictionary<Model, List<ModelComponent>> components)) {
                if (components.TryGetValue(actor, out List<ModelComponent> list) && list.Remove(component)) {
                    ActorComponents<T>.InvokeRemove(actor, component);
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