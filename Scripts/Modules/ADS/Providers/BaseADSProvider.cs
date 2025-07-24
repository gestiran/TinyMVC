// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

#if GOOGLE_ADS_MOBILE
    using System;
    using Cysharp.Threading.Tasks;
    using GoogleMobileAds.Api;
    
    namespace TinyMVC.Modules.ADS.Providers {
        public abstract class BaseADSProvider {
            public bool isLoading { get; protected set; }
            
            protected Action _onCloseCallback;
            protected bool _isClose;
            protected bool _isLoadSuccess;
            private int _waitIteration;
            
            protected readonly Func<AdRequest> _getGoogleRequest;
            
            protected const int _WAIT_LOADING_REFRESH_MILLISECOND = 33;
            private const int _WAIT_NETWORK_CHECK_MILLISECOND = 5000;
            private const int _WAIT_AFTER_ERROR_MILLISECOND = 20000;
            
            protected BaseADSProvider(Func<AdRequest> getGoogleRequest) {
                _waitIteration = 0;
                _getGoogleRequest = getGoogleRequest;
            }
            
            public void Activate(ADSRating rating, bool isNeedLoad) {
                SetRating(rating);
                
                if (isNeedLoad) {
                    Load();
                }
            }
            
            public abstract bool IsLoaded();
            
            public abstract void Load();
            
            protected virtual void OnLoadSuccess() { }
            
            protected abstract void SetRating(ADSRating rating);
            
            protected abstract void Remove();
            
            protected abstract bool IsNeedCloseCallback();
            
            protected async UniTask WaitLoading() {
                while (isLoading) {
                    await UniTask.Delay(_WAIT_LOADING_REFRESH_MILLISECOND, DelayType.Realtime, PlayerLoopTiming.Update);
                }
                
                if (_isLoadSuccess) {
                    _waitIteration = 0;
                    OnLoadSuccess();
                } else {
                    WaitingBeforeLoading().Forget();
                }
            }
            
            protected async UniTask WaitingClosed(Action onClose) {
                _isClose = false;
                
                while (!_isClose) {
                    await UniTask.Delay(_WAIT_LOADING_REFRESH_MILLISECOND, DelayType.Realtime, PlayerLoopTiming.Update);
                }
                
                Remove();
                onClose();
                
                if (IsNeedCloseCallback()) {
                #if DEBUG_ADS
                    UnityEngine.Debug.LogError($"{GetType().Name}.WaitingClosed: Closed!");
                #endif
                    _onCloseCallback();
                }
            }
            
            protected async UniTask WaitingNetwork() {
                await UniTask.Delay(_WAIT_NETWORK_CHECK_MILLISECOND, DelayType.Realtime, PlayerLoopTiming.Update);
                Load();
            }
            
            private async UniTask WaitingBeforeLoading() {
                await UniTask.Delay(_WAIT_AFTER_ERROR_MILLISECOND * ++_waitIteration, DelayType.Realtime, PlayerLoopTiming.Update);
                
                if (!IsLoaded()) {
                    Load();
                }
            }
        }
    }
#endif