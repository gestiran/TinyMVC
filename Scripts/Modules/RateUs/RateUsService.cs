// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#if GOOGLE_PLAY_REVIEW && GOOGLE_PLAY_COMMON
using Google.Play.Common;
using Google.Play.Review;
#endif

namespace TinyMVC.Modules.RateUs {
    public static class RateUsService {
        public static event Action onShow;
        public static RateUsParameters data { get; private set; }
        public static bool isNeedShow { get; private set; }
        
        private static bool _isFirstShow;
        private static int _timer;
        
        static RateUsService() {
            data = RateUsParameters.LoadFromResources();
            isNeedShow = RateUsSaveUtility.LoadIsNeedShow(data.isEnableRateUs);
            
            if (isNeedShow == false) {
                return;
            }
            
            _isFirstShow = RateUsSaveUtility.LoadIsFirstShow();
            
            if (_isFirstShow) {
                _timer = Mathf.Max(RateUsSaveUtility.LoadToRateUsTime(data.remoteConfig.firstShowDelay), data.remoteConfig.afterAppStartDelay);
            } else {
                _timer = Mathf.Max(RateUsSaveUtility.LoadToRateUsTime(data.remoteConfig.otherShowDelay), data.remoteConfig.afterAppStartDelay);
            }
            
            TimerProcess();
        }
        
        public static void AddTime(int minutes) => _timer += minutes;
        
        public static bool TryOpenWindow() {
            if (IsNeedShow()) {
                OpenWindow();
                return true;
            }
            
            return false;
        }
        
        public static bool IsNeedShow() => isNeedShow && _timer <= 0;
        
        public static bool TryRateUs() {
            if (isNeedShow) {
                isNeedShow = false;
                RateUsSaveUtility.SaveIsNeedShow(false);
                return false;
            }
            
            RateUs();
            return true;
        }
        
        public static async void RateUs() {
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
        
        public static void SendToMail(string email, string title) => Application.OpenURL($"mailto:{email}?subject={title}&body={MyEscapeURL("")}");
        
        public static void OnClose() {
            _isFirstShow = false;
            RateUsSaveUtility.SaveIsFirstShow(_isFirstShow);
            _timer = data.remoteConfig.otherShowDelay;
        }
        
    #if GOOGLE_PLAY_REVIEW && GOOGLE_PLAY_COMMON
        
        private static async Task<bool> TryShowForm() {
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
        
        private static string MyEscapeURL(string url) => UnityWebRequest.EscapeURL(url).Replace("+", "%20");
        
        private static void OpenWindow() => onShow?.Invoke();
        
        private static async void TimerProcess() {
            while (Application.isPlaying) {
                await Task.Delay(60000);
                
                if (_timer > 0) {
                    _timer--;
                    RateUsSaveUtility.SaveToRateUsTime(_timer);
                }
            }
        }
    }
}