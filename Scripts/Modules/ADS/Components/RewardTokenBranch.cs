using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TinyMVC.Modules.ADS.Components {
    [DisallowMultipleComponent]
    public sealed class RewardTokenBranch : MonoBehaviour {
        [field: SerializeField]
        public GameObject active;
        
        [field: SerializeField]
        public GameObject inactive;
        
        [field: SerializeField]
        public GameObject token;
        
        [field: SerializeField]
        public int tokensPrice = 1;
        
    #if GOOGLE_ADS_MOBILE
        private void Awake() => UpdateAds(false);
        
        private void OnEnable() {
            UpdateAds(API<ADSModule>.module.IsLoadRewarded());
            
            API<ADSModule>.module.onRewardActiveStateChange += UpdateAds;
            API<ADSTokenModule>.module.onCountChanged += UpdateTokens;
        }
        
        private void OnDisable() {
            StopAllCoroutines();
            API<ADSModule>.module.onRewardActiveStateChange -= UpdateAds;
            API<ADSTokenModule>.module.onCountChanged -= UpdateTokens;
        }
        
        private void UpdateAds(bool isLoadedReward) {
            try {
                if (gameObject.activeInHierarchy) {
                    StopAllCoroutines();
                    StartCoroutine(UpdateProcess(isLoadedReward, API<ADSTokenModule>.module.tokenCount));
                } else {
                    UpdateState(isLoadedReward, API<ADSTokenModule>.module.tokenCount);
                }
            } catch (Exception exception) {
                Debug.LogWarning(new Exception("RewardTokenBranch.UpdateAds", exception));
            }
        }
        
        private void UpdateTokens(int tokensCount) {
            try {
                if (gameObject.activeInHierarchy) {
                    StopAllCoroutines();
                    StartCoroutine(UpdateProcess(API<ADSModule>.module.IsLoadRewarded(), tokensCount));
                } else {
                    UpdateState(API<ADSModule>.module.IsLoadRewarded(), tokensCount);
                }
            } catch (Exception exception) {
                Debug.LogWarning(new Exception("RewardTokenBranch.UpdateTokens", exception));
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateState(bool isLoadedReward, int tokensCount) {
            if (tokensCount >= tokensPrice) {
                ToToken();
            } else {
                if (isLoadedReward) {
                    ToActive();
                } else {
                    ToInactive();
                }
            }
        }
        
        private bool IsValidState() {
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ToActive() {
            active.SetActive(true);
            inactive.SetActive(false);
            token.SetActive(false);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ToInactive() {
            active.SetActive(false);
            inactive.SetActive(true);
            token.SetActive(false);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ToToken() {
            active.SetActive(false);
            inactive.SetActive(false);
            token.SetActive(true);
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