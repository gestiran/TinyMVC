using System;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace TinyMVC.Modules.IAP {
    public abstract class CodelessIAPModule : IApplicationModule {
        public IAPParameters data { get; }
        public static event Action<string> onBuySuccess;
        
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
        
    #if UNITY_PURCHASING_FAKE
        public void ConfiscateAllPurchased() {
            BuyHandler[] handlers = CreateNonConsumableHandlers();
            
            for (int handlerId = 0; handlerId < handlers.Length; handlerId++) {
                if (handlers[handlerId].IsPurchased()) {
                    handlers[handlerId].Confiscate();   
                }
            }
        }
    #endif
        
        protected abstract BuyHandler[] CreateNonConsumableHandlers();
        
        internal static void OnBuySuccess(string productId) => onBuySuccess?.Invoke(productId);
    }
}