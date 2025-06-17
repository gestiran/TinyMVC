using TinyMVC.Boot;
using TinyMVC.Views;

namespace TinyMVC.Dependencies.Components {
    public abstract class Actor<T> : Actor where T : View {
        public new T view { get; internal set; }
    }
    
    public abstract class Actor : Model {
        public View view { get; internal set; }
        
        internal protected override void AddComponentInternal<T>(T component) {
            ProjectContext.components.Add(this, component);
            componentsList.Add(typeof(T).FullName, component);
        }
        
        internal protected override void RemoveComponentInternal(string key) {
            if (componentsList.TryGetValue(key, out ModelComponent component)) {
                ProjectContext.components.Remove(this, component);
                componentsList.RemoveByKey(key);
            }
        }
    }
}