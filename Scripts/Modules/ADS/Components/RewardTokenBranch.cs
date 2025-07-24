// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if GOOGLE_ADS_MOBILE
using System;
using System.Collections;
#endif

namespace TinyMVC.Modules.ADS.Components {
    internal sealed class RewardTokenBranch : RewardBranch {
    #if ODIN_INSPECTOR
        [field: Required]
    #endif
        [field: SerializeField]
        public GameObject token;
        
    #if ODIN_INSPECTOR
        [field: MinValue(1)]
    #endif
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
        
        private void UpdateState(bool isLoadedReward, int tokensCount) {
            if (tokensCount >= tokensPrice) {
                ToToken();
            } else {
                base.UpdateState(isLoadedReward);
            }
        }
        
        protected override void ToActive() {
            base.ToActive();
            token.SetActive(false);
        }
        
        protected override void ToInactive() {
            base.ToInactive();
            token.SetActive(false);
        }
        
        private void ToToken() {
            active.SetActive(false);
            inactive.SetActive(false);
            token.SetActive(true);
        }
        
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