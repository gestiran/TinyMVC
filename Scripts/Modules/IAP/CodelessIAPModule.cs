namespace TinyMVC.Modules.IAP {
    public abstract class CodelessIAPModule : IApplicationModule {
        public IAPParameters data { get; }
        
        protected CodelessIAPModule() => data = IAPParameters.LoadFromResources();
        
        public virtual void Init() {
            #if UNITY_EDITOR
            StartRestoreProcess();
            #else
            StartRestoreProcess();
            #endif
        }
        
        private async void StartRestoreProcess() {
            IAPRestoreHandler restoreHandler = new IAPRestoreHandler();
            await restoreHandler.RestoreProcess(CreateNonConsumableHandlers());
        }
        
        protected abstract BuyHandler[] CreateNonConsumableHandlers();
    }
}