#if GOOGLE_ADS_MOBILE
    using System;
    using System.Threading.Tasks;
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
            
            protected async void WaitLoading() {
                while (isLoading) {
                    await Task.Delay(_WAIT_LOADING_REFRESH_MILLISECOND);
                }
                
                if (_isLoadSuccess) {
                    _waitIteration = 0;
                    OnLoadSuccess();
                } else {
                    WaitingBeforeLoading();
                }
            }
            
            protected async void WaitingClosed(Action onClose) {
                _isClose = false;
                
                while (!_isClose) {
                    await Task.Delay(_WAIT_LOADING_REFRESH_MILLISECOND);
                }
                
                Remove();
                onClose();
                _onCloseCallback();
            }
            
            protected async void WaitingNetwork() {
                await Task.Delay(_WAIT_NETWORK_CHECK_MILLISECOND);
                Load();
            }
            
            private async void WaitingBeforeLoading() {
                await Task.Delay(_WAIT_AFTER_ERROR_MILLISECOND * ++_waitIteration);
                
                if (!IsLoaded()) {
                    Load();
                }
            }
        }
    }
#endif