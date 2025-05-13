using UnityEngine;
using Sirenix.OdinInspector;

#if GOOGLE_ADS_MOBILE
using System;
using System.Collections;
#endif

namespace TinyMVC.Modules.ADS.Components {
    [DisallowMultipleComponent]
    internal class RewardBranch : MonoBehaviour {
        [field: SerializeField, Required]
        public GameObject active;
        
        [field: SerializeField, Required]
        public GameObject inactive;
        
        protected Coroutine _updateProcess;
        
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
                if (gameObject.activeInHierarchy == false) {
                    return;
                }
                
                StopUpdateProcess();
                _updateProcess = StartCoroutine(UpdateProcess(isLoaded));
            } catch (Exception exception) {
                Debug.LogWarning(exception);
            }
        }
        
        protected void StopUpdateProcess() {
            if (_updateProcess != null) {
                StopCoroutine(_updateProcess);
            }
            
            _updateProcess = null;
        }
        
        protected void UpdateState(bool isLoaded) {
            if (isLoaded) {
                ToActive();
            } else {
                ToInactive();
            }
        }
        
        protected virtual bool IsValidState() {
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
        
        protected virtual void ToActive() {
            active.SetActive(true);
            inactive.SetActive(false);
        }
        
        protected virtual void ToInactive() {
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