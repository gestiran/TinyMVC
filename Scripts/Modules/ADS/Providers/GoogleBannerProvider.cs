#if GOOGLE_ADS_MOBILE
using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace TinyMVC.Modules.ADS.Providers {
    public sealed class GoogleBannerProvider {
        public bool isVisible { get; private set; }
        public bool isLoaded { get; private set; }
        
        private string _bannerId;
        private BannerView _bannerView;
        
        private readonly string _kidsId;
        private readonly string _generalId;
        private readonly Func<AdRequest> _getGoogleRequest;
        
        public GoogleBannerProvider(Func<AdRequest> getGoogleRequest, string kidsId, string generalId) {
            _bannerId = kidsId;
            _kidsId = kidsId;
            _generalId = generalId;
            _getGoogleRequest = getGoogleRequest;
            isVisible = true;
            isLoaded = false;
        }
        
        public void Activate(ADSRating rating, bool isNeedLoad) {
            if (string.IsNullOrEmpty(_bannerId)) {
                _bannerId = _kidsId;
            }
            
            SetRating(rating);
            
            if (isNeedLoad) {
                Load();
            }
        }
        
        private void Load() {
            #if DEBUG_ADS
            Debug.LogError("GoogleBannerProvider.Load");
            #endif
            
            if (_bannerView == null) {
                _bannerView = Create();
            }
            
            _bannerView.LoadAd(_getGoogleRequest.Invoke());
        }
        
        public void Show() {
            if (isLoaded == false) {
                #if DEBUG_ADS
                Debug.LogError("GoogleBannerProvider.Show: Isn't loaded!");
                #endif
                
                Load();
            } else {
                #if DEBUG_ADS
                Debug.LogError("GoogleBannerProvider.Show: Success!");
                #endif
                
                _bannerView.Show();
            }
            
            isVisible = true;
        }
        
        public void Hide() {
            if (_bannerView != null) {
                #if DEBUG_ADS
                Debug.LogError("GoogleBannerProvider.Hide: Success!");
                #endif
                _bannerView.Hide();
                Destroy();
            } else {
                #if DEBUG_ADS
                Debug.LogError("GoogleBannerProvider.Hide: Isn't initialized!");
                #endif
            }
            
            isVisible = false;
        }
        
        private BannerView Create() {
            BannerView bannerView = new BannerView(_bannerId, AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);
            bannerView.OnBannerAdLoaded += OnLoaded;
            bannerView.OnBannerAdLoadFailed += OnLoadFailed;
            return bannerView;
        }
        
        private void Destroy() {
            _bannerView.OnBannerAdLoaded -= OnLoaded;
            _bannerView.OnBannerAdLoadFailed -= OnLoadFailed;
            _bannerView.Destroy();
            _bannerView = null;
            isLoaded = false;
        }
        
        private void OnLoaded() {
            #if DEBUG_ADS
            Debug.LogError("GoogleBannerProvider.OnLoaded: Success!");
            #endif
            isLoaded = true;
            _bannerView.Show();
        }
        
        private void OnLoadFailed(LoadAdError error) {
            #if DEBUG_ADS
            Debug.LogError($"GoogleBannerProvider.OnLoadFailed: {error.GetMessage()}");
            #endif
            
            isLoaded = false;
        }
        
        private void SetRating(ADSRating rating) => _bannerId = rating.tagForChildDirectedTreatment ? _kidsId : _generalId;
    }
}
#endif