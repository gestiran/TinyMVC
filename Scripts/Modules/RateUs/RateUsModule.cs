using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#if GOOGLE_PLAY_REVIEW && GOOGLE_PLAY_COMMON
using Google.Play.Common;
using Google.Play.Review;
#endif

namespace TinyMVC.Modules.RateUs {
    public sealed class RateUsModule : IApplicationModule {
        public event Action showRateUs;
        public RateUsParameters data { get; private set; }
        public bool isNeedShow { get; private set; }
        
        private bool _isFirstShow;
        private int _toShowTimer;
        
        public RateUsModule() {
            data = RateUsParameters.LoadFromResources();
            isNeedShow = RateUsSaveUtility.LoadIsNeedShow(data.isEnableRateUs);
            
            if (isNeedShow == false) {
                return;
            }
            
            _isFirstShow = RateUsSaveUtility.LoadIsFirstShow();
            
            if (_isFirstShow) {
                _toShowTimer = Mathf.Max(RateUsSaveUtility.LoadToRateUsTime(data.remoteConfig.firstShowDelay), data.remoteConfig.afterAppStartDelay);
            } else {
                _toShowTimer = Mathf.Max(RateUsSaveUtility.LoadToRateUsTime(data.remoteConfig.otherShowDelay), data.remoteConfig.afterAppStartDelay);
            }
            
            TimerProcess();
        }
        
        public void AddTime(int minutes) => _toShowTimer += minutes;
        
        public bool TryOpenWindow() {
            if (IsNeedShow()) {
                OpenWindow();
                return true;
            }
            
            return false;
        }
        
        public bool IsNeedShow() => isNeedShow && _toShowTimer <= 0;
        
        public async void RateUs() {
            if (isNeedShow) {
                isNeedShow = false;
                RateUsSaveUtility.SaveIsNeedShow(false);
            }
            
        #if GOOGLE_PLAY_REVIEW && GOOGLE_PLAY_COMMON
            
            if (await TryShowForm()) {
                // TODO : Need test rate-us showing!
                // await Task.Yield();
                // Debug.LogError(Application.isFocused);
            } else {
                Application.OpenURL($"market://details?id={Application.identifier}");
            }
            
        #else
            Application.OpenURL($"market://details?id={Application.identifier}");
        #endif
        }
        
        public void SendToMail(string email, string title) => Application.OpenURL($"mailto:{email}?subject={title}&body={MyEscapeURL("")}");
        
        public void OnClose() {
            _isFirstShow = false;
            RateUsSaveUtility.SaveIsFirstShow(_isFirstShow);
            _toShowTimer = data.remoteConfig.otherShowDelay;
        }
        
    #if GOOGLE_PLAY_REVIEW && GOOGLE_PLAY_COMMON
        
        private async Task<bool> TryShowForm() {
            try {
                ReviewManager manager = new ReviewManager();
                PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> request = manager.RequestReviewFlow();
                
                do {
                    await Task.Yield();
                } while (request.IsDone == false);
                
                if (request.Error == ReviewErrorCode.NoError) {
                    PlayAsyncOperation<VoidResult, ReviewErrorCode> launch = manager.LaunchReviewFlow(request.GetResult());
                    
                    do {
                        await Task.Yield();
                    } while (launch.IsDone == false);
                    
                    if (launch.Error == ReviewErrorCode.NoError) {
                        return true;
                    }
                }
            } catch (Exception) {
                // Do nothing
            }
            
            return false;
        }
        
    #endif
        
        private string MyEscapeURL(string url) => UnityWebRequest.EscapeURL(url).Replace("+", "%20");
        
        private void OpenWindow() => showRateUs?.Invoke();
        
        private async void TimerProcess() {
            while (Application.isPlaying) {
                await Task.Delay(60000);
                
                if (_toShowTimer > 0) {
                    _toShowTimer--;
                    RateUsSaveUtility.SaveToRateUsTime(_toShowTimer);
                }
            }
        }
    }
}