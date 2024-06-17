namespace TinyMVC.Modules {
    public static class API<TModule> where TModule : class, IApplicationModule, new() {
        public static TModule module { get; private set; }
        
        static API() => module = new TModule();
    }
    
    public static class API {
        public const string SAVE_GROUP = "API";
    }
}