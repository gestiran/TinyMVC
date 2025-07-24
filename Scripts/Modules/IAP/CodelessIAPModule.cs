// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Threading.Tasks;

#if UNITY_PURCHASING
using UnityEngine;
using UnityEngine.Purchasing;
#endif

namespace TinyMVC.Modules.IAP {
    public abstract class CodelessIAPModule : IApplicationModule {
        public IAPParameters data { get; }
        public static event Action<string> onBuySuccess;
        
        private bool _isTimeout;
        
        private const int _INIT_RETRY_DELAY = 1000;
        private const int _INIT_WAIT_DELAY = 60000;
        
        protected CodelessIAPModule() => data = IAPParameters.LoadFromResources();
        
        public Task Initialize() => Initialize(_ => { });
        
        public async Task Initialize(Action<IAPStatus> onComplete) {
        #if UNITY_PURCHASING
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                onComplete.Invoke(IAPStatus.FailedNetworkNotReachable);
                
                do {
                    await Task.Delay(_INIT_RETRY_DELAY);
                } while (Application.internetReachability == NetworkReachability.NotReachable);
            }
            
            await Task.WhenAny(InitializeCodeless(), WaitLimit());
            
            if (CodelessIAPStoreListener.initializationComplete) {
                onComplete.Invoke(IAPStatus.Success);
            } else if (_isTimeout) {
                onComplete.Invoke(IAPStatus.FailedTimeout);
            } else {
                onComplete.Invoke(IAPStatus.FailedInternal);
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
        
        private async Task InitializeCodeless() {
        #if UNITY_PURCHASING
            CodelessIAPStoreListener _ = CodelessIAPStoreListener.Instance;
            
            do {
                await Task.Delay(_INIT_RETRY_DELAY);
            } while (CodelessIAPStoreListener.initializationComplete == false);
        #endif
        }
        
        private async Task WaitLimit() {
            await Task.Delay(_INIT_WAIT_DELAY);
            _isTimeout = true;
        }
    }
}