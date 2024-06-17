using System;
using System.Threading.Tasks;

#if GOOGLE_ADS_MOBILE
using UnityEngine;
using TinyMVC.Modules.ADS.Providers;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
#endif

namespace TinyMVC.Modules.ADS {
    public abstract class GoogleADS {
        public bool isReady { get; private set; }
        public ADSParameters data { get; private set; }
        public ConsentState consentState { get; protected set; }
        public abstract bool isNoADS { get; protected set; }
        
        public bool isCanShowConsent;
        
        #if GOOGLE_ADS_MOBILE
        protected GoogleInterstitialProvider _googleInterstitial;
        protected GoogleRewardedProvider _googleReward;
        protected GoogleBannerProvider _banner;

        private ADSRating _rating;
        
        #endif
        
        private bool _isADSInitialized;
        
        #if GOOGLE_ADS_MOBILE
        #if DEBUG_ADS
        private string _consentFailedLog;
        #endif
        
        private const string _MAX_AD_CONTENT_RATING = "max_ad_content_rating";
        private const int _CONSENT_CHECK_DELAY = 5000;
        
        #endif
        
        public enum ConsentState : byte {
            Process,
            Success,
            Failed
        }
        
        protected GoogleADS() {
            _isADSInitialized = false;
            isReady = false;
            data = ADSParameters.LoadFromResources();
        }
        
        public virtual void Init(bool isLoadInterstitial, bool isLoadReward, bool isLoadBanner) {
            #if UNITY_ANDROID
            ADSParameters.Config config = data.android;
            #elif UNITY_IOS
            ADSParameters.Config config = data.ios;
            #endif
            
            #if GOOGLE_ADS_MOBILE
            _googleInterstitial = new GoogleInterstitialProvider(GetAdRequest, config.kids.interstitial, config.general.interstitial);
            _googleReward = new GoogleRewardedProvider(GetAdRequest, config.kids.reward, config.general.reward);
            _banner = new GoogleBannerProvider(GetAdRequest, config.kids.banner, config.general.banner);
            #endif
            
            if (ADSSaveUtility.HasSavedAge()) {
                SetADSRating((byte)ADSSaveUtility.LoadAge());
                StartModule(isLoadInterstitial, isLoadReward, isLoadBanner);
            }
        }
        
        public void SetADSRating(byte age) {
            if (!ADSSaveUtility.HasSavedAge()) {
                ADSSaveUtility.SaveAge(age);
            }
            
            #if GOOGLE_ADS_MOBILE
            if (_rating != null) {
                #if DEBUG_ADS
                Debug.LogError("GoogleADS.SetADSRating: Already set!");
                #endif
                return;
            }
            
            _rating = new ADSRating(age);
            
            if (isReady) {
                try {
                    RequestConfiguration configuration = new RequestConfiguration();
                    
                    configuration.TagForUnderAgeOfConsent = _rating.tagForUnderAgeOfConsent;
                    configuration.TagForChildDirectedTreatment = _rating.GetTagForChildDirectedTreatment();
                    configuration.MaxAdContentRating = _rating.GetRating();
                    
                    MobileAds.SetRequestConfiguration(configuration);
                } catch (Exception) {
                    // Do nothing
                }
            }
            
            #endif
            
            #if DEBUG_ADS
            Debug.LogError($"GoogleADS.SetADSRating: Saved {age} as age!");
            #endif
        }
        
        private void StartModule(bool isLoadInterstitial, bool isLoadReward, bool isLoadBanner) {
            if (!ADSSaveUtility.HasSavedAge()) {
                #if DEBUG_ADS
                Debug.LogError("GoogleADS.StartModule: Hasn't saved age!");
                #endif
                return;
            }
            
            #if DEBUG_ADS
            Debug.LogError("GoogleADS.StartModule");
            #endif
            
            #if UNITY_EDITOR
            InitializeADS(isLoadInterstitial, isLoadReward, isLoadBanner);
            
            return;
            #endif
            
            #if GOOGLE_ADS_MOBILE
            SendConsentRequest(isLoadInterstitial, isLoadReward, isLoadBanner);
            #endif
        }
        
        #if UNITY_ANDROID
        public string GetGAID() {
            string advertisingID = "";
            
            try {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass advertisingIdClient = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
                AndroidJavaObject adInfo = advertisingIdClient.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", currentActivity);
                
                advertisingID = adInfo.Call<string>("getId");
            } catch (Exception exception) {
                Debug.LogError($"GoogleADS.GAID: {exception.Message}");
                return null;
            }
            
            return advertisingID;
        }
        
        #endif
        
        protected virtual void ActivateADS(bool isLoadInterstitial, bool isLoadReward, bool isLoadBanner) {
            #if DEBUG_ADS && UNITY_ANDROID
            Debug.LogError($"GoogleADS.ActivateAds: GAID: {GetGAID()}");
            #endif
            
            #if GOOGLE_ADS_MOBILE
            _googleInterstitial.Activate(_rating, isLoadInterstitial);
            _googleReward.Activate(_rating, isLoadReward);
            _banner.Activate(_rating, isLoadBanner);
            #endif
            
            isReady = true;
        }
        
        
        #if GOOGLE_ADS_MOBILE
        private void OnConsentInfoUpdated(FormError error) {
            if (error != null) {
                #if DEBUG_ADS
                _consentFailedLog = $"GoogleADS.OnConsentInfoUpdated: {error}";
                #endif
                consentState = ConsentState.Failed;
                
                return;
            }
            
            #if DEBUG_ADS
            Debug.LogError("GoogleADS.OnConsentInfoUpdated: Success!");
            #endif
            
            if (!isCanShowConsent) {
                #if DEBUG_ADS
                _consentFailedLog = "GoogleADS.OnConsentInfoUpdated: Can't show!";
                #endif
                consentState = ConsentState.Failed;
                
                return;
            }
            
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                #if DEBUG_ADS
                _consentFailedLog = "GoogleADS.OnConsentInfoUpdated: No network connection!";
                #endif
                consentState = ConsentState.Failed;
                
                return;
            }
            
            try {
                ConsentForm.LoadAndShowConsentFormIfRequired(OnFormDismissed);
            } catch (Exception e) {
                #if DEBUG_ADS
                _consentFailedLog = $"GoogleADS.OnConsentInfoUpdated: {e}";
                #endif
                consentState = ConsentState.Failed;
            }
        }
        
        private void SendConsentRequest(bool isLoadInterstitial, bool isLoadReward, bool isLoadBanner) {
            #if DEBUG_ADS
            Debug.LogError("GoogleADS.SendConsentRequest: Start");
            #endif
            
            consentState = ConsentState.Process;
            Action sendRequest = () => SendConsentRequest(isLoadInterstitial, isLoadReward, isLoadBanner);
            WaitConsentAsync(() => CheckConsentResult(sendRequest, () => InitializeADS(isLoadInterstitial, isLoadReward, isLoadBanner)));
            
            if (!isCanShowConsent) {
                #if DEBUG_ADS
                _consentFailedLog = "GoogleADS.SendConsentRequest: Can't show!";
                #endif
                consentState = ConsentState.Failed;
                
                return;
            }
            
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                #if DEBUG_ADS
                _consentFailedLog = "GoogleADS.SendConsentRequest: No network connection!";
                #endif
                consentState = ConsentState.Failed;
                
                return;
            }
            
            try {
                ConsentRequestParameters consentRequest = new ConsentRequestParameters();
                consentRequest.TagForUnderAgeOfConsent = _rating.tagForUnderAgeOfConsent == TagForUnderAgeOfConsent.True;
                
                ConsentInformation.Update(consentRequest, OnConsentInfoUpdated);
            } catch (Exception e) {
                #if DEBUG_ADS
                _consentFailedLog = $"GoogleADS.SendConsentRequest - {e}";
                #endif
                consentState = ConsentState.Failed;
            }
        }
        
        private void OnFormDismissed(FormError error) {
            if (error != null) {
                #if DEBUG_ADS
                _consentFailedLog = $"GoogleADS.OnFormDismissed: {error}";
                #endif
                return;
            }
            
            try {
                if (!ConsentInformation.CanRequestAds()) {
                    #if DEBUG_ADS
                    _consentFailedLog = "GoogleADS.OnFormDismissed: Can`t request!";
                    #endif
                    consentState = ConsentState.Failed;
                    
                    return;
                }
                
                #if DEBUG_ADS
                Debug.LogError("GoogleADS.OnFormDismissed: Success!");
                #endif
                
                consentState = ConsentState.Success;
            } catch (Exception e) {
                #if DEBUG_ADS
                _consentFailedLog = $"GoogleADS.OnFormDismissed: {e}";
                #endif
                consentState = ConsentState.Failed;
            }
        }
        
        private async void CheckConsentResult(Action sendRequest, Action onSuccess) {
            if (consentState == ConsentState.Failed) {
                #if DEBUG_ADS
                Debug.LogError($"GoogleADS.CheckConsentResult - Consent Failed!\n{_consentFailedLog}");
                #endif
                
                await Task.Delay(_CONSENT_CHECK_DELAY);
                sendRequest();
                
                return;
            }
            
            onSuccess();
        }
        #endif
        
        private void InitializeADS(bool isLoadInterstitial, bool isLoadReward, bool isLoadBanner) {
            #if DEBUG_ADS
            Debug.LogError("GoogleADS.InitializeAds");
            #endif
            
            #if GOOGLE_ADS_MOBILE
            try {
                MobileAds.Initialize(_ => _isADSInitialized = true);
            } catch (Exception) {
                #if DEBUG_ADS
                Debug.LogError("GoogleADS.InitializeAds: Success!");
                #endif
                return;
            }
            #else
            _isADSInitialized = true;
            #endif
            
            WaitInitializeCompletedAsync(() => ActivateADS(isLoadInterstitial, isLoadReward, isLoadBanner));
        }
        
        #if GOOGLE_ADS_MOBILE
        private async void WaitConsentAsync(Action action) {
            while (consentState == ConsentState.Process) {
                await Task.Delay(33);
            }
            
            await Task.Delay(33);
            action();
        }
        #endif
        
        private async void WaitInitializeCompletedAsync(Action action) {
            while (!_isADSInitialized) {
                await Task.Delay(33);
            }
            
            await Task.Delay(33);
            action();
        }
        
        #if GOOGLE_ADS_MOBILE
        private AdRequest GetAdRequest() {
            AdRequest request = new AdRequest();
            
            request.Keywords.Add("unity-admob-sample");
            request.Extras.Add(_MAX_AD_CONTENT_RATING, _rating.rating.ToString());
            
            return request;
        }
        #endif
    }
}