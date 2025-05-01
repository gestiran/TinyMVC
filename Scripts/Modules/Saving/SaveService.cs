namespace TinyMVC.Modules.Saving {
    public static partial class SaveService {
        private static readonly SaveHandler _handler;
        
        static SaveService() {
            SaveParameters parameters = SaveParameters.LoadFromResources();
            
            if (parameters != null) {
                _handler = new SaveHandler(parameters.rootDirectory, parameters.versionLabel);
            } else {
                _handler = new SaveHandler(SaveParameters.ROOT_DIRECTORY, SaveParameters.VERSION_LABEL);
            }
        }
        
        public static bool HasGroup(params string[] group) => _handler.HasGroup(group);
        
        public static bool Has(string key, params string[] group) => _handler.Has(key, group);
        
        public static bool Has(string key) => _handler.Has(key);
        
        public static string[] GetGroups(params string[] group) => _handler.GetGroups(group);
        
        public static string[] GetData(params string[] group) => _handler.GetData(group);
        
        public static void Save<T>(T value, string key) => _handler.Save(value, key);
        
        public static void Save<T>(T value, string key, params string[] group) => _handler.Save(value, key, group);
        
        public static bool TryLoad<T>(out T result, string key) => _handler.TryLoad(out result, key);
        
        public static bool TryLoad<T>(out T result, string key, params string[] group) => _handler.TryLoad(out result, key, group);
        
        public static T Load<T>(string key) => _handler.Load(default(T), key);
        
        public static T Load<T>(T defaultValue, string key) => _handler.Load(defaultValue, key);
        
        public static T Load<T>(string key, params string[] group) => _handler.Load(default(T), key, group);
        
        public static T Load<T>(T defaultValue, string key, params string[] group) => _handler.Load(defaultValue, key, group);
        
        public static void DeleteGroup(params string[] group) => _handler.DeleteGroup(group);
        
        public static void Delete(string key, params string[] group) => _handler.Delete(key, group);
        
        public static void Delete(string key) => _handler.Delete(key);
    }
}