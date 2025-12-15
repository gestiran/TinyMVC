namespace TinyMVC.Modules.Saving.Reactive.Handlers {
    public interface ISaveHandler<T> {
        public void Save(T value, string key);
        
        public void Save(T value, string key, params string[] group);
        
        public T Load(T defaultValue, string key);
        
        public T Load(T defaultValue, string key, params string[] group);
    }
}