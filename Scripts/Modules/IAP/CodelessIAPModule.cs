using System.Threading.Tasks;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace TinyMVC.Modules.IAP {
    public abstract class CodelessIAPModule : IApplicationModule {
        public IAPParameters data { get; }
        
        private const int _INIT_RETRY_DELAY = 1000;
        
        protected CodelessIAPModule() => data = IAPParameters.LoadFromResources();
        
        public async Task Initialize() {
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                return;
            }
            
        #if UNITY_PURCHASING
            // Start initialization
            CodelessIAPStoreListener _ = CodelessIAPStoreListener.Instance;
            
            while (Application.isPlaying) {
                if (CodelessIAPStoreListener.initializationComplete) {
                    break;
                }
                
                await Task.Delay(_INIT_RETRY_DELAY);
            }
        #endif
        }
        
        public virtual async Task StartRestoreProcess() {
            IAPRestoreHandler restoreHandler = new IAPRestoreHandler();
            await restoreHandler.RestoreProcess(CreateNonConsumableHandlers());
        }
        
        protected abstract BuyHandler[] CreateNonConsumableHandlers();
    }
}