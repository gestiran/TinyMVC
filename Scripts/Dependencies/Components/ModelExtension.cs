using System.Collections.Generic;
using TinyMVC.Boot;

namespace TinyMVC.Dependencies.Components {
    public static class ModelExtension {
        public static bool AddComponent<TModel, TComponent>(this TModel model, TComponent component) where TModel : Model where TComponent : ModelComponent {
            string key = typeof(TComponent).FullName;
            
            if (model.components.ContainsKey(key)) {
                return false;
            }
            
            model.components.Add(key, component);
            ProjectContext.components.Add(model, component);
            return true;
        }
        
        public static bool RemoveComponent<TModel, TComponent>(this TModel model) where TModel : Model where TComponent : ModelComponent {
            string key = typeof(TComponent).FullName;
            
            if (model.components.TryGetValue(key, out ModelComponent component)) {
                model.components.Remove(key);
                ProjectContext.components.Remove(model, component);
                return true;
            }
            
            return false;
        }
        
        public static bool RemoveComponent<TModel, TComponent>(this TModel model, TComponent component) where TModel : Model where TComponent : ModelComponent {
            string key = component.GetType().FullName;
            
            if (model.components.TryGetValue(key, out ModelComponent target)) {
                model.components.Remove(key);
                ProjectContext.components.Remove(model, target);
                return true;
            }
            
            return false;
        }
        
        public static void RemoveComponents<TModel, TComponent>(this TModel model, TComponent components) where TModel : Model where TComponent : IEnumerable<ModelComponent> {
            foreach (ModelComponent component in components) {
                string key = component.GetType().FullName;
                
                if (model.components.TryGetValue(key, out ModelComponent target)) {
                    model.components.Remove(key);
                    ProjectContext.components.Remove(model, target);
                }
            }
        }
        
        public static bool IsHaveComponent<T>(this Model model) {
            foreach (ModelComponent other in model.components.root.Values) {
                if (other is T) {
                    return true;
                }
            }
            
            return false;
        }
        
        public static bool IsHaveComponent<T>(this Model model, T component) {
            foreach (ModelComponent other in model.components.root.Values) {
                if (other.Equals(component)) {
                    return true;
                }
            }
            
            return false;
        }
        
        public static bool TryGetComponent<T>(this Model model, out T component) {
            foreach (ModelComponent other in model.components.root.Values) {
                if (other is T target) {
                    component = target;
                    return true;
                }
            }
            
            component = default;
            return false;
        }
        
        public static IEnumerable<T> ForEach<T>(this Model model) {
            foreach (ModelComponent component in model.components.ForEachValues()) {
                if (component is T target) {
                    yield return target;
                }
            }
        }
        
        public static IEnumerable<(T1, T2)> ForEach<T1, T2>(this Model model) {
            IEnumerable<ModelComponent> values = model.components.ForEachValues();
            
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
        
        public static IEnumerable<(T1, T2, T3)> ForEach<T1, T2, T3>(this Model model) {
            IEnumerable<ModelComponent> values = model.components.ForEachValues();
            
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
        
        public static IEnumerable<(T1, T2, T3, T4)> ForEach<T1, T2, T3, T4>(this Model model) {
            IEnumerable<ModelComponent> values = model.components.ForEachValues();
            
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
    }
}