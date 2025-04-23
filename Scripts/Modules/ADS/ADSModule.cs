using System;
using UnityEngine;

#if GOOGLE_ADS_MOBILE
using System.Threading.Tasks;
#endif

namespace TinyMVC.Modules.ADS {
    public sealed class ADSModule : GoogleADS, IApplicationModule {
        public override bool isNoADS { get; protected set; }
        public bool isAdsVisible { get; private set; }
        
    #if GOOGLE_ADS_MOBILE
        public bool isVisibleBanner => _banner != null && _banner.isVisible;
        public bool isInterstitialEnable => _withoutInterstitialTime <= 0;
    #else
        public bool isVisibleBanner => false;
    #endif
        
        public event Action onInterstitialShow;
        public event Action onInterstitialShowFailed;
        public event Action onRewardShow;
        public event Action onRewardShowFailed;
        public event Action onBannerShow;
        public event Action onBannerHide;
        public event Action<bool> noAdsStateChange;
        
        private int _withoutInterstitialTime;
        private int _bannerRewardsCount;
        private AsyncCancellation _bannerProcessCancellation;
        
        public ADSModule() : base() {
            isCanShowConsent = true;
            isNoADS = ADSSaveUtility.LoadIsNoADS();
        }
        
        public override void Init(bool isLoadInterstitial, bool isLoadReward, bool isLoadBanner) {
        #if DEBUG_ADS
            Debug.LogError($"ADSModule.Init: interstitial: {isLoadInterstitial}, reward: {isLoadReward}, banner: {isLoadBanner}");
        #endif
            
            base.Init(isLoadInterstitial, isLoadReward, isLoadBanner);
        }
        
        protected override void ActivateADS() {
            base.ActivateADS();
            
            _withoutInterstitialTime = Mathf.Max(ADSSaveUtility.LoadWithoutInterstitialTime(data.remoteConfig.beforeFirstInterstitial),
                                                 data.remoteConfig.beforeAppStartInterstitial);
        #if GOOGLE_ADS_MOBILE
            if (IsActiveADS()) {
                if (ADSSaveUtility.LoadBannerVisibility()) {
                    _banner.Show();
                } else {
                    _banner.Hide();
                }
                
                if (_bannerProcessCancellation != null) {
                    _bannerProcessCancellation.Cancel();
                    _bannerProcessCancellation = null;
                }
                
                AsyncCancellation cancellation = new AsyncCancellation();
                UpdateBannerProcess(ADSSaveUtility.LoadRemainingBannerTime(data.remoteConfig.bannerUpdateTime), cancellation);
                _bannerProcessCancellation = cancellation;
                UpdateInterstitialProcess();
            }
        #endif
        }
        
        public bool HasSavedAge() => ADSSaveUtility.HasSavedAge();
        
        public void LoadInterstitial() {
            if (data.fullNoADSMode) {
                return;
            }
            
            if (isNoADS) {
            #if DEBUG_ADS
                Debug.LogError("ADSModule.LoadInterstitial: NoAds active, operation canceled!");
            #endif
                return;
            }
            
            if (!isReady) {
            #if DEBUG_ADS
                Debug.LogError("ADSModule.LoadInterstitial: Isn't ready!");
            #endif
                return;
            }
            
        #if GOOGLE_ADS_MOBILE
            if (_googleInterstitial.IsLoaded()) {
            #if DEBUG_ADS
                Debug.LogError("ADSModule.LoadInterstitial: Already loaded!");
            #endif
                return;
            }
        #endif
            
        #if DEBUG_ADS
            Debug.LogError("ADSModule.LoadInterstitial: Success!");
        #endif
            
        #if GOOGLE_ADS_MOBILE
            _googleInterstitial.Load();
        #endif
        }
        
        public bool IsLoadRewarded() {
            if (data.fullNoADSMode) {
                return true;
            }
            
            if (!isReady) {
                return false;
            }
            
        #if GOOGLE_ADS_MOBILE
            return _googleReward.IsLoaded();
        #else
            return true;
        #endif
        }
        
        public void LoadReward() {
            if (data.fullNoADSMode) {
                return;
            }
            
            if (!isReady) {
            #if DEBUG_ADS
                Debug.LogError("ADSModule.LoadReward: Isn't ready!");
            #endif
                return;
            }
            
        #if GOOGLE_ADS_MOBILE
            if (_googleReward.IsLoaded()) {
            #if DEBUG_ADS
                Debug.LogError($"ADSModule.LoadReward: Already loaded!");
            #endif
                return;
            }
            
            _googleReward.Load();
        #endif
        }
        
        public void BuyNoADS() {
        #if DEBUG_ADS
            Debug.LogError($"ADSModule.BuyNoADS");
        #endif
            
            if (isNoADS == false) {
                isNoADS = true;
                ADSSaveUtility.SaveIsNoADS(isNoADS);
                noAdsStateChange?.Invoke(true);
            }
            
        #if GOOGLE_ADS_MOBILE
            if (_banner != null) {
                _banner.Hide();
            }
            
            if (_bannerProcessCancellation != null) {
                _bannerProcessCancellation.Cancel();
                _bannerProcessCancellation = null;
            }
            
            _bannerRewardsCount = 0;
            ADSSaveUtility.SaveBannerRewardsCount(_bannerRewardsCount);
            ADSSaveUtility.SaveRemainingBannerTime(data.remoteConfig.bannerUpdateTime);
        #endif
        }
        
        public void RemoveNoADS() {
        #if DEBUG_ADS
            Debug.LogError($"ADSModule.RemoveNoADS");
        #endif
            
            if (isNoADS) {
                isNoADS = false;
                ADSSaveUtility.SaveIsNoADS(isNoADS);
                noAdsStateChange?.Invoke(false);
            }
            
        #if GOOGLE_ADS_MOBILE
            if (_bannerProcessCancellation != null) {
                _bannerProcessCancellation.Cancel();
                _bannerProcessCancellation = null;
            }
            
            AsyncCancellation cancellation = new AsyncCancellation();
            UpdateBannerProcess(ADSSaveUtility.LoadRemainingBannerTime(data.remoteConfig.bannerUpdateTime), cancellation);
            _bannerProcessCancellation = cancellation;
        #endif
        }
        
        public void OnPurchaseTokens() {
            _withoutInterstitialTime += data.remoteConfig.tokensPurchaseInterstitialDisable;
            _bannerRewardsCount += 5;
            
            ADSSaveUtility.SaveBannerRewardsCount(_bannerRewardsCount);
            ADSSaveUtility.SaveWithoutInterstitialTime(_withoutInterstitialTime);
        }
        
        public bool TryShowInterstitial() {
            bool isShowed = TryShowInterstitial(() => { });
            
            return isShowed;
        }
        
        public bool TryShowInterstitial(Action onClose) => TryShowInterstitialProcess(onClose);
        
        public bool TryShowReward(Action onSuccess) => TryShowReward(onSuccess, () => { });
        
        public bool TryShowReward(Action onSuccess, Action onFailed) => TryShowRewardProcess(onSuccess, onFailed);
        
        private bool TryShowInterstitialProcess(Action onClose) {
            isAdsVisible = true;
            
            if (data.fullNoADSMode) {
                onClose();
                isAdsVisible = false;
                return false;
            }
            
            if (!isReady) {
                onClose();
                isAdsVisible = false;
            #if DEBUG_ADS
                Debug.LogError("ADSModule.TryShowInterstitialProcess: Is not ready!");
            #endif
                
                return false;
            }
            
            if (IsNeedInterstitial()) {
                onInterstitialShow?.Invoke();
                
            #if GOOGLE_ADS_MOBILE
                if (_googleInterstitial.IsLoaded()) {
                #if DEBUG_ADS
                    Debug.LogError("ADSModule.TryShowInterstitialProcess: Show interstitial!");
                #endif
                    
                    return _googleInterstitial.TryShow(() => LoadNextInterstitialAndClose(onClose));
                }
                
                _googleInterstitial.Load();
            #endif
                
            #if DEBUG_ADS
                Debug.LogError("ADSModule.TryShowInterstitialProcess: Isn't loaded!");
            #endif
            }
        #if DEBUG_ADS
            else {
                Debug.LogError("ADSModule.TryShowInterstitialProcess: Isn't need!");
            }
        #endif
            
            onClose();
            isAdsVisible = false;
            return true;
        }
        
        private bool TryShowRewardProcess(Action onSuccess, Action onFailed) {
            if (data.fullNoADSMode) {
                onSuccess();
                return true;
            }
            
            if (!isReady) {
                onFailed();
                
            #if DEBUG_ADS
                Debug.LogError("ADSModule.TryShowRewardProcess: Is not ready!");
            #endif
                
                return false;
            }
            
        #if GOOGLE_ADS_MOBILE
            if (_googleReward.IsLoaded()) {
            #if DEBUG_ADS
                Debug.LogError("ADSModule.TryShowRewardProcess: Show first reward!");
            #endif
                
                if (onRewardShow != null) {
                    onSuccess += onRewardShow;    
                }
                
                if (onRewardShowFailed != null) {
                    onFailed += onRewardShowFailed;
                }
                
                return _googleReward.TryShow(() => OnShowingReward(onSuccess), onFailed);
            }
            
            _googleReward.Load();
        #else
            onSuccess();
            
            return true;
        #endif
        #if DEBUG_ADS
            Debug.LogError("ADSModule.TryShowRewardProcess: Failed!");
        #endif
            
            onFailed();
            
            return false;
        }
        
        public bool IsCanShowInterstitial() {
        #if GOOGLE_ADS_MOBILE
            return IsNeedInterstitial() && _googleInterstitial.IsLoaded();
        #else
            return IsNeedInterstitial();
        #endif
        }
        
        private bool IsNeedInterstitial() {
            if (data.fullNoADSMode) {
                return false;
            }
            
            if (isNoADS) {
                return false;
            }
            
            if (_withoutInterstitialTime > 0) {
                onInterstitialShowFailed?.Invoke();
                
                return false;
            }
            
            return true;
        }
        
    #if GOOGLE_ADS_MOBILE
        private async void UpdateInterstitialProcess() {
        #if DEBUG_ADS
            Debug.LogError($"ADSModule.UpdateInterstitialProcess: time: {_withoutInterstitialTime}");
        #endif
            
            while (Application.isPlaying) {
                await Task.Delay(60000);
                
                if (IsActiveADS() == false) {
                    break;
                }
                
                if (_withoutInterstitialTime > 0) {
                    _withoutInterstitialTime--;
                    ADSSaveUtility.SaveWithoutInterstitialTime(_withoutInterstitialTime);
                    
                    if (_withoutInterstitialTime < 1 && _googleInterstitial.isLoading == false && _googleInterstitial.IsLoaded() == false) {
                        _googleInterstitial.Load();
                    }
                }
            }
        }
        
        private async void UpdateBannerProcess(int updateTime, AsyncCancellation cancellation) {
            _bannerRewardsCount = ADSSaveUtility.LoadBannerRewardsCount();
            
        #if DEBUG_ADS
            Debug.LogError($"ADSModule.UpdateBannerProcess: time: {updateTime}");
        #endif
            
            while (cancellation.isCancel == false) {
                for (int time = updateTime - 1; time >= 0; time--) {
                    await Task.Delay(60000);
                    
                    if (cancellation.isCancel) {
                        return;
                    }
                    
                    ADSSaveUtility.SaveRemainingBannerTime(time);
                }
                
                updateTime = data.remoteConfig.bannerUpdateTime;
                
                UpdateBannerState();
                
                _bannerRewardsCount = 0;
                ADSSaveUtility.SaveBannerRewardsCount(_bannerRewardsCount);
                ADSSaveUtility.SaveRemainingBannerTime(updateTime);
            }
        }
        
        private void UpdateBannerState() {
        #if DEBUG_ADS
            Debug.LogError($"ADSModule.UpdateBannerState: rewards: {_bannerRewardsCount}, need: {data.remoteConfig.bannerRewardsLimit}");
        #endif
            
            if (_bannerRewardsCount < data.remoteConfig.bannerRewardsLimit) {
                TryShowBanner();
            } else {
                TryHideBanner();
            }
        }
        
        private void TryShowBanner() {
            ADSSaveUtility.SaveBannerVisibility(true);
            
            if (_banner.isVisible) {
                return;
            }
            
            _banner.Show();
            onBannerShow?.Invoke();
        }
        
        private void TryHideBanner() {
            ADSSaveUtility.SaveBannerVisibility(false);
            
            if (_banner.isVisible == false) {
                return;
            }
            
            _banner.Hide();
            onBannerHide?.Invoke();
        }
        
        private void LoadNextInterstitialAndClose(Action onClose) {
            _googleInterstitial.Load();
            onClose.Invoke();
            isAdsVisible = false;
        }
        
        private bool IsActiveADS() {
            if (data.fullNoADSMode) {
                return false;
            }
            
            if (isNoADS) {
                return false;
            }
            
            return true;
        }
        
        private void OnShowingReward(Action onComplete) {
            _withoutInterstitialTime += data.remoteConfig.rewardInterstitialDisable;
            
            ADSSaveUtility.SaveBannerRewardsCount(++_bannerRewardsCount);
            ADSSaveUtility.SaveWithoutInterstitialTime(_withoutInterstitialTime);
            
            onComplete();
        }
    #endif
    }
}