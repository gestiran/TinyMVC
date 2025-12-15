namespace TinyMVC.Modules.Saving.Reactive.Handlers {
    public sealed class DefaultSaveHandler<T> : ISaveHandler<T> {
        public static DefaultSaveHandler<T> instance { get; private set; }
        
        static DefaultSaveHandler() => instance = new DefaultSaveHandler<T>();
        
        private DefaultSaveHandler() { }
        
        public void Save(T value, string key) => SaveService.Save(value, key);
        
        public void Save(T value, string key, params string[] group) => SaveService.Save(value, key, group);
        
        public T Load(T defaultValue, string key) => SaveService.Load(defaultValue, key);
        
        public T Load(T defaultValue, string key, params string[] group) => SaveService.Load(defaultValue, key, group);
    }
}