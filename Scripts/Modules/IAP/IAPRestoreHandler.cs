using System.Threading.Tasks;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace TinyMVC.Modules.IAP {
    public sealed class IAPRestoreHandler {
        private const int _BETWEEN_RESTORE_TRIES = 1000;
        
        public async Task RestoreProcess(BuyHandler[] handlers) {
        #if UNITY_PURCHASING
            CodelessIAPStoreListener store = CodelessIAPStoreListener.Instance;
        #endif
            
            while (Application.isPlaying) {
            #if UNITY_PURCHASING
                if (!CodelessIAPStoreListener.initializationComplete) {
                    await Task.Delay(_BETWEEN_RESTORE_TRIES);
                    
                    continue;
                }
            #endif
                
            #if UNITY_EDITOR
                IAPParameters parameters = IAPParameters.LoadFromResources();
                
                if (parameters.isUsingLastPurchases) {
                    for (int handlerId = 0; handlerId < handlers.Length; handlerId++) {
                        handlers[handlerId].CheckAndRestoreDebug(parameters.debugPurchases);
                        await Task.Yield();
                    }
                }
                
            #else
                for (int handlerId = 0; handlerId < handlers.Length; handlerId++) {
                    handlers[handlerId].CheckAndRestore(store);
                    await Task.Yield();
                }
            #endif
                
                break;
            }
        }
    }
}