namespace TinyMVC.ApplicationLevel {
    public static class API<TModule> where TModule : struct, IApplicationModule {
        public static TModule module { get; private set; }

        static API() {
            module = new TModule();
            module.Initialize();
        }
    }
}