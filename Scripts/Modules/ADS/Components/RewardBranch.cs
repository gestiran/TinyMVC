using System;
using System.Collections;
using UnityEngine;

namespace TinyMVC.Modules.ADS.Components {
    [DisallowMultipleComponent]
    public sealed class RewardBranch : MonoBehaviour {
        [field: SerializeField]
        public GameObject active;
        
        [field: SerializeField]
        public GameObject inactive;
        
        #if GOOGLE_ADS_MOBILE
        
        private void OnEnable() {
            UpdateRequest(API<ADSModule>.module.IsLoadRewarded());
            API<ADSModule>.module.onRewardActiveStateChange += UpdateRequest;
        }
        
        private void OnDisable() => API<ADSModule>.module.onRewardActiveStateChange -= UpdateRequest;
        
        private void UpdateRequest(bool isLoaded) {
            try {
                StopAllCoroutines();
                UpdateState(isLoaded);
            } catch (Exception exception) {
                Debug.LogWarning(exception);
                
                if (gameObject.activeInHierarchy) {
                    StartCoroutine(UpdateAfterFrame(isLoaded));
                }
            }
        }
        
        private void UpdateState(bool isLoaded) {
            if (isLoaded) {
                ToActive();
            } else {
                ToInactive();
            }
        }
        
        private void ToActive() {
            active.SetActive(true);
            inactive.SetActive(false);
        }
        
        private void ToInactive() {
            active.SetActive(false);
            inactive.SetActive(true);
        }
        
        private IEnumerator UpdateAfterFrame(bool isLoaded) {
            yield return new WaitForSecondsRealtime(0.5f);
            
            try {
                UpdateState(isLoaded);
            } catch (Exception exception) {
                Debug.LogWarning(exception);
            }
        }
        
        #endif
    }
}