#if GOOGLE_ADS_MOBILE
    using System;
    using GoogleMobileAds.Api;
    using UnityEngine;
    
    namespace TinyMVC.Modules.ADS.Providers {
        public sealed class GoogleInterstitialProvider : BaseADSProvider {
            private string _interstitialId;
            private InterstitialAd _interstitial;
            
            private readonly string _kidsId;
            private readonly string _generalId;
            
            public GoogleInterstitialProvider(Func<AdRequest> getGoogleRequest, string kidsId, string generalId) : base(getGoogleRequest) {
                _interstitialId = kidsId;
                _kidsId = kidsId;
                _generalId = generalId;
            }
            
            public override bool IsLoaded() {
                if (!_isLoadSuccess) {
                #if DEBUG_ADS
                    Debug.LogError("GoogleInterstitialProvider.IsLoaded: Loading already in progress!");
                #endif
                    return false;
                }
                
                if (_interstitial == null) {
                #if DEBUG_ADS
                    Debug.LogError("GoogleInterstitialProvider.IsLoaded: Interstitial is null!");
                #endif
                    
                    _isLoadSuccess = false;
                    
                    return false;
                }
                
                try {
                    bool result = _interstitial.CanShowAd();
                #if DEBUG_ADS
                    Debug.LogError($"GoogleInterstitialProvider.IsLoaded: Success, result: {result}!");
                #endif
                    return result;
                } catch (Exception e) {
                #if DEBUG_ADS
                    Debug.LogError($"GoogleInterstitialProvider.IsLoaded: {e}");
                #endif
                    
                    _isLoadSuccess = false;
                    
                    return false;
                }
            }
            
            public override void Load() {
                if (isLoading) {
                #if DEBUG_ADS
                    Debug.LogError("GoogleInterstitialProvider.Load: Failed, loading already in progress!");
                #endif
                    return;
                }
                
                if (string.IsNullOrEmpty(_interstitialId)) {
                    _interstitialId = _kidsId;
                }
                
                if (Application.internetReachability == NetworkReachability.NotReachable) {
                    WaitingNetwork();
                #if DEBUG_ADS
                    Debug.LogError("GoogleInterstitialProvider.Load: Network not reachable!");
                #endif
                } else {
                    isLoading = true;
                    WaitLoading();
                    
                    try {
                        InterstitialAd.Load(_interstitialId, _getGoogleRequest(), OnInterstitialLoaded);
                    #if DEBUG_ADS
                        Debug.LogError("GoogleInterstitialProvider.Load: Success!");
                    #endif
                    } catch (Exception e) {
                    #if DEBUG_ADS
                        Debug.LogError($"GoogleInterstitialProvider.Load: {e}");
                    #endif
                        OnInterstitialLoaded(null, null);
                    }
                }
            }
            
            public bool TryShow(Action onClose) {
                if (!_isLoadSuccess) {
                #if DEBUG_ADS
                    Debug.LogError("GoogleInterstitialProvider.TryShow: Isn't loaded!");
                #endif
                    return false;
                }
                
                try {
                    if (!_interstitial.CanShowAd()) {
                        return false;
                    }
                    
                    _onCloseCallback = onClose;
                    WaitingClosed(() => { });
                    _interstitial.OnAdFullScreenContentClosed += OnAdClosed;
                    _interstitial.Show();
                #if DEBUG_ADS
                    Debug.LogError("GoogleInterstitialProvider.TryShow: Success!");
                #endif
                    return true;
                } catch (Exception e) {
                #if DEBUG_ADS
                    Debug.LogError($"GoogleInterstitialProvider.TryShow: {e}");
                #endif
                    
                    _isLoadSuccess = false;
                    
                    return false;
                }
            }
            
            protected override void SetRating(ADSRating rating) => _interstitialId = rating.tagForChildDirectedTreatment ? _kidsId : _generalId;
            
            protected override void Remove() {
            #if DEBUG_ADS
                Debug.LogError("GoogleInterstitialProvider.Remove");
            #endif
                _interstitial.Destroy();
                _interstitial = null;
                _isLoadSuccess = false;
            }
            
            protected override bool IsNeedCloseCallback() => true;
            
            private void OnInterstitialLoaded(InterstitialAd interstitial, LoadAdError error) {
                if (error != null || interstitial == null) {
                #if DEBUG_ADS
                    Debug.LogError($"GoogleInterstitialProvider.OnInterstitialLoaded: {error}");
                #endif
                    _isLoadSuccess = false;
                    isLoading = false;
                    
                    return;
                }
                
            #if DEBUG_ADS
                Debug.LogError("GoogleInterstitialProvider.OnInterstitialLoaded: Success!");
            #endif
                _isLoadSuccess = true;
                isLoading = false;
                _interstitial = interstitial;
            }
            
            private void OnAdClosed() => _isClose = true;
        }
    }
#endif