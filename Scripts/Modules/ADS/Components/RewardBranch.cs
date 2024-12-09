using UnityEngine;

#if GOOGLE_ADS_MOBILE
using System;
using System.Collections;
using System.Runtime.CompilerServices;
#endif

namespace TinyMVC.Modules.ADS.Components {
    [DisallowMultipleComponent]
    public sealed class RewardBranch : MonoBehaviour {
        [field: SerializeField]
        public GameObject active;
        
        [field: SerializeField]
        public GameObject inactive;
        
    #if GOOGLE_ADS_MOBILE
        private void Awake() => UpdateRequest(false);
        
        private void OnEnable() {
            UpdateRequest(API<ADSModule>.module.IsLoadRewarded());
            API<ADSModule>.module.onRewardActiveStateChange += UpdateRequest;
        }
        
        private void OnDisable() {
            StopAllCoroutines();
            API<ADSModule>.module.onRewardActiveStateChange -= UpdateRequest;
        }
        
        private void UpdateRequest(bool isLoaded) {
            try {
                if (gameObject.activeInHierarchy) {
                    StopAllCoroutines();
                    StartCoroutine(UpdateProcess(isLoaded));
                } else {
                    UpdateState(isLoaded);
                }
            } catch (Exception exception) {
                Debug.LogWarning(exception);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateState(bool isLoaded) {
            if (isLoaded) {
                ToActive();
            } else {
                ToInactive();
            }
        }
        
        private bool IsValidState() {
            bool activeState = active.gameObject.activeSelf;
            bool inactiveState = inactive.gameObject.activeSelf;
            
            if (activeState && inactiveState) {
                return false;
            }
            
            if (activeState == false && inactiveState == false) {
                return false;
            }
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ToActive() {
            active.SetActive(true);
            inactive.SetActive(false);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ToInactive() {
            active.SetActive(false);
            inactive.SetActive(true);
        }
        
        private IEnumerator UpdateProcess(bool isLoaded) {
            do {
                try {
                    UpdateState(isLoaded);
                } catch (Exception exception) {
                    Debug.LogWarning(exception);
                }
                
                yield return new WaitForEndOfFrame();
            } while (IsValidState() == false);
        }
        
    #endif
    }
}