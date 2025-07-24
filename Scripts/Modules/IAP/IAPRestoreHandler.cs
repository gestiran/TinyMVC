// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Threading.Tasks;
using UnityEngine;

#if UNITY_PURCHASING
using System;
using UnityEngine.Purchasing;
#endif

namespace TinyMVC.Modules.IAP {
    public sealed class IAPRestoreHandler {
        private const int _RESTORE_CHECK_DELAY = 1000;
        
        public async Task RestoreProcess(BuyHandler[] handlers) {
            do {
                await Task.Delay(_RESTORE_CHECK_DELAY);
                
                if (Application.internetReachability == NetworkReachability.NotReachable) {
                    continue;
                }
                
            #if UNITY_PURCHASING
                try {
                    if (CodelessIAPStoreListener.initializationComplete == false) {
                        // Retry initialization
                        CodelessIAPStoreListener _ = CodelessIAPStoreListener.Instance;
                        continue;
                    }
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception("IAPRestoreHandler.RestoreProcess", exception));
                    continue;
                }
                
                break;
            #endif
            } while (Application.isPlaying);
            
        #if UNITY_EDITOR
            IAPParameters parameters = IAPParameters.LoadFromResources();
            
            if (parameters.isUsingLastPurchases) {
                for (int handlerId = 0; handlerId < handlers.Length; handlerId++) {
                    handlers[handlerId].CheckAndRestoreDebug(parameters.debugPurchases);
                    await Task.Yield();
                }
            }
        #elif UNITY_PURCHASING && !UNITY_PURCHASING_FAKE
            CodelessIAPStoreListener store = CodelessIAPStoreListener.Instance;
            
            for (int handlerId = 0; handlerId < handlers.Length; handlerId++) {
                handlers[handlerId].CheckAndRestore(store);
                await Task.Yield();
            }
        #endif
        }
    }
}