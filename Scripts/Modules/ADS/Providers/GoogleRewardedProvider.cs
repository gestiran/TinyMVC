#if GOOGLE_ADS_MOBILE
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using GoogleMobileAds.Api;
    using UnityEngine;
    
    namespace TinyMVC.Modules.ADS.Providers {
        public sealed class GoogleRewardedProvider : BaseADSProvider {
            public event Action<bool> onActiveStateChange;
            
            private string _rewardId;
            private RewardedAd _rewardedAd;
            
            private Action _onRewardedCallback;
            private bool _isRewarded;
            private CancellationTokenSource _cancellation;
            
            private readonly string _kidsId;
            private readonly string _generalId;
            
            public GoogleRewardedProvider(Func<AdRequest> getGoogleRequest, string kidsId, string generalId) : base(getGoogleRequest) {
                _rewardId = kidsId;
                _kidsId = kidsId;
                _generalId = generalId;
            }
            
            public override bool IsLoaded() {
                if (!_isLoadSuccess) {
                #if DEBUG_ADS
                    Debug.LogError("GoogleRewardedProvider.IsLoaded: Loading already in progress!");
                #endif
                    return false;
                }
                
                if (_rewardedAd == null) {
                #if DEBUG_ADS
                    Debug.LogError("GoogleRewardedProvider.IsLoaded: RewardedAd is null!");
                #endif
                    
                    _isLoadSuccess = false;
                    
                    return false;
                }
                
                try {
                    bool result = _rewardedAd.CanShowAd();
                    
                #if DEBUG_ADS
                    Debug.LogError($"GoogleRewardedProvider.IsLoaded: Success, result: {result}!");
                #endif
                    
                    return result;
                } catch (Exception exception) {
                #if DEBUG_ADS
                    Debug.LogError($"GoogleRewardedProvider.IsLoaded: {exception}");
                #endif
                    _isLoadSuccess = false;
                    
                    return false;
                }
            }
            
            public override void Load() {
                if (isLoading) {
                #if DEBUG_ADS
                    Debug.LogError("GoogleRewardedProvider.Load: Failed, loading already in progress!");
                #endif
                    return;
                }
                
                if (Application.internetReachability == NetworkReachability.NotReachable) {
                    WaitingNetwork().Forget();
                #if DEBUG_ADS
                    Debug.LogError("GoogleRewardedProvider.Load: Network not reachable!");
                #endif
                } else {
                    isLoading = true;
                    WaitLoading().Forget();
                    
                    try {
                        RewardedAd.Load(_rewardId, _getGoogleRequest(), OnRewardedLoaded);
                        
                    #if DEBUG_ADS
                        Debug.LogError("GoogleRewardedProvider.Load: Success!");
                    #endif
                    } catch (Exception e) {
                    #if DEBUG_ADS
                        Debug.LogError($"GoogleRewardedProvider.Load: {e}");
                    #endif
                        
                        OnRewardedLoaded(null, null);
                    }
                }
            }
            
            public bool TryShow(Action onRewarded, Action onClose) {
                if (!_isLoadSuccess) {
                #if DEBUG_ADS
                    Debug.LogError("GoogleRewardedProvider.TryShow: Isn't loaded!");
                #endif
                    return false;
                }
                
                if (string.IsNullOrEmpty(_rewardId)) {
                    _rewardId = _kidsId;
                }
                
                try {
                    if (!_rewardedAd.CanShowAd()) {
                        return false;
                    }
                    
                    _onRewardedCallback = onRewarded;
                    _onCloseCallback = onClose;
                    
                    if (_cancellation != null) {
                        _cancellation.Cancel();
                        _cancellation.Dispose();
                    }
                    
                    _cancellation = new CancellationTokenSource();
                    
                    WaitingRewarded(_cancellation.Token).Forget();
                    WaitingClosed(Load).Forget();
                    
                    _rewardedAd.OnAdFullScreenContentClosed += OnAdClosed;
                    _rewardedAd.Show(OnUserEarnedReward);
                    
                #if DEBUG_ADS
                    Debug.LogError("GoogleRewardedProvider.TryShow: Success!");
                #endif
                    
                    onActiveStateChange?.Invoke(false);
                    
                    return true;
                } catch (Exception e) {
                #if DEBUG_ADS
                    Debug.LogError($"GoogleRewardedProvider.TryShow: {e}");
                #endif
                    
                    _isLoadSuccess = false;
                    
                    return false;
                }
            }
            
            protected override void OnLoadSuccess() {
                base.OnLoadSuccess();
                onActiveStateChange?.Invoke(true);
            }
            
            protected override void SetRating(ADSRating rating) => _rewardId = rating.tagForChildDirectedTreatment ? _kidsId : _generalId;
            
            protected override void Remove() {
            #if DEBUG_ADS
                Debug.LogError("GoogleRewardedProvider.Remove");
            #endif
                
                if (_rewardedAd != null) {
                    _rewardedAd.Destroy();
                }
                
                _rewardedAd = null;
                _isLoadSuccess = false;
            }
            
            protected override bool IsNeedCloseCallback() => _isRewarded == false;
            
            private void OnRewardedLoaded(RewardedAd rewardedAd, LoadAdError error) {
                if (error != null || rewardedAd == null) {
                #if DEBUG_ADS
                    Debug.LogError($"GoogleRewardedProvider.OnRewardedLoaded: {error}");
                #endif
                    _isLoadSuccess = false;
                    isLoading = false;
                    
                    return;
                }
                
            #if DEBUG_ADS
                Debug.LogError("GoogleRewardedProvider.OnRewardedLoaded: Success!");
            #endif
                
                _isLoadSuccess = true;
                isLoading = false;
                _rewardedAd = rewardedAd;
            }
            
            private void OnAdClosed() {
                _isClose = true;
            #if UNITY_EDITOR
                _isRewarded = true;
            #endif
            }
            
            private void OnUserEarnedReward(Reward reward) => _isRewarded = true;
            
            private async UniTask WaitingRewarded(CancellationToken cancellation) {
                _isRewarded = false;
                
                while (!IsNeedRewardCallback()) {
                    await UniTask.Delay(_WAIT_LOADING_REFRESH_MILLISECOND, DelayType.Realtime, PlayerLoopTiming.Update, cancellation);
                }
                
            #if DEBUG_ADS
                UnityEngine.Debug.LogError($"GoogleRewardedProvider.WaitingRewarded: Rewarded!");
            #endif
                
                _onRewardedCallback();
            }
            
            private bool IsNeedRewardCallback() {
            #if UNITY_EDITOR_IOS || UNITY_IOS
                return _isRewarded && _isClose;
            #endif
                return _isRewarded;
            }
        }
    }
#endif