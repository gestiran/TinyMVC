namespace TinyMVC.ApplicationLevel {
    public static class API<TModule> where TModule : class, IApplicationModule, new() {
        public static TModule module { get; private set; }

        static API() {
            module = new TModule();
            module.Initialize();
        }
    }
}