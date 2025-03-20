using UnityEngine;
using Sirenix.OdinInspector;

#if GOOGLE_ADS_MOBILE
using System;
using System.Collections;
using System.Runtime.CompilerServices;
#endif

namespace TinyMVC.Modules.ADS.Components {
    internal sealed class RewardTokenBranch : RewardBranch {
        [field: SerializeField, Required]
        public GameObject token;
        
        [field: SerializeField, MinValue(1)]
        public int tokensPrice = 1;
        
    #if GOOGLE_ADS_MOBILE
        private void Awake() => UpdateAds(false);
        
        private void OnEnable() {
            UpdateAds(API<ADSModule>.module.IsLoadRewarded());
            
            API<ADSModule>.module.onRewardActiveStateChange += UpdateAds;
            API<ADSTokenModule>.module.onCountChanged += UpdateTokens;
        }
        
        private void OnDisable() {
            StopUpdateProcess();
            
            API<ADSModule>.module.onRewardActiveStateChange -= UpdateAds;
            API<ADSTokenModule>.module.onCountChanged -= UpdateTokens;
        }
        
        private void UpdateAds(bool isLoadedReward) {
            try {
                if (gameObject.activeInHierarchy == false) {
                    return;
                }
                
                StopUpdateProcess();
                _updateProcess = StartCoroutine(UpdateProcess(isLoadedReward, API<ADSTokenModule>.module.tokenCount));
            } catch (Exception exception) {
                Debug.LogWarning(new Exception("RewardTokenBranch.UpdateAds", exception));
            }
        }
        
        private void UpdateTokens(int tokensCount) {
            try {
                if (gameObject.activeInHierarchy == false) {
                    return;
                }
                
                StopUpdateProcess();
                _updateProcess = StartCoroutine(UpdateProcess(API<ADSModule>.module.IsLoadRewarded(), tokensCount));
            } catch (Exception exception) {
                Debug.LogWarning(new Exception("RewardTokenBranch.UpdateTokens", exception));
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateState(bool isLoadedReward, int tokensCount) {
            if (tokensCount >= tokensPrice) {
                ToToken();
            } else {
                base.UpdateState(isLoadedReward);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void ToActive() {
            base.ToActive();
            token.SetActive(false);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void ToInactive() {
            base.ToInactive();
            token.SetActive(false);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ToToken() {
            active.SetActive(false);
            inactive.SetActive(false);
            token.SetActive(true);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool IsValidState() {
            if (base.IsValidState() == false) {
                return false;
            }
            
            bool activeState = active.gameObject.activeSelf;
            bool inactiveState = inactive.gameObject.activeSelf;
            bool tokenState = token.gameObject.activeSelf;
            
            if (activeState) {
                if (inactiveState) {
                    return false;
                }
                
                if (tokenState) {
                    return false;
                }
            } else if (inactiveState) {
                if (tokenState) {
                    return false;
                }
            }
            
            return activeState || inactiveState || tokenState;
            
        }
        
        private IEnumerator UpdateProcess(bool isLoadedReward, int tokensCount) {
            do {
                try {
                    UpdateState(isLoadedReward, tokensCount);
                } catch (Exception exception) {
                    Debug.LogWarning(new Exception("RewardTokenBranch.UpdateProcess", exception));
                }
                
                yield return new WaitForEndOfFrame();
            } while (IsValidState() == false);
        }
        
    #endif
    }
}