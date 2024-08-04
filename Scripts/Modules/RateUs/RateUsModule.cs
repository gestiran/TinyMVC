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
            _isFirstShow = RateUsSaveUtility.LoadIsFirstShow();
            
            if (isNeedShow == false) {
                return;
            }
            
            if (_isFirstShow) {
                _toShowTimer = Mathf.Max(RateUsSaveUtility.LoadToRateUsTime(data.firstShowDelay), data.afterAppStartDelay);
            } else {
                _toShowTimer = Mathf.Max(RateUsSaveUtility.LoadToRateUsTime(data.otherShowDelay), data.afterAppStartDelay);
            }
            
            TimerProcess();
        }
        
        public void AddTime(int minutes) => _toShowTimer += minutes;
        
        public bool TryOpenWindow() {
            if (IsNeedShow() == false) {
                return false;
            }
            
            OpenWindow();
            return true;
        }
        
        public bool IsNeedShow() {
            if (isNeedShow == false) {
                return false;
            }
            
            return _toShowTimer <= 0;
        }
        
        public async void RateUs() {
            if (isNeedShow) {
                isNeedShow = false;
                RateUsSaveUtility.SaveIsNeedShow(isNeedShow);
            }
            
            #if GOOGLE_PLAY_REVIEW && GOOGLE_PLAY_COMMON
            
            try {
                ReviewManager reviewManager = new ReviewManager();
                
                PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> request = reviewManager.RequestReviewFlow();
                
                if (request.IsDone == false) {
                    await Task.Yield();
                }
                
                if (request.Error == ReviewErrorCode.NoError && request.IsSuccessful) {
                    PlayAsyncOperation<VoidResult, ReviewErrorCode> launch = reviewManager.LaunchReviewFlow(request.GetResult());
                    
                    if (launch.IsDone == false) {
                        await Task.Yield();
                    }
                    
                    if (launch.Error == ReviewErrorCode.NoError) {
                        return;
                    }
                }
            } catch (Exception) {
                // Do nothing
            }
            
            Application.OpenURL($"market://details?id={Application.identifier}");
            
            #else
            Application.OpenURL($"market://details?id={Application.identifier}");
            #endif
        }
        
        public void SendToMail(string email, string title) => Application.OpenURL($"mailto:{email}?subject={title}&body={MyEscapeURL("")}");
        
        public void OnClose() {
            _isFirstShow = false;
            RateUsSaveUtility.SaveIsFirstShow(_isFirstShow);
            _toShowTimer = data.otherShowDelay;
        }
        
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