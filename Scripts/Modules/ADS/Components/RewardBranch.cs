using UnityEngine;

namespace TinyMVC.Modules.ADS.Components {
    [DisallowMultipleComponent]
    public sealed class RewardBranch : MonoBehaviour {
        [field: SerializeField]
        public GameObject active;
        
        [field: SerializeField]
        public GameObject inactive;
        
        #if GOOGLE_ADS_MOBILE
        
        private void Start() {
            UpdateState(API<ADSModule>.module.IsLoadRewarded());
            API<ADSModule>.module.onRewardActiveStateChange += UpdateState;
        }
        
        private void OnDestroy() => API<ADSModule>.module.onRewardActiveStateChange -= UpdateState;
        
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
        
        #endif
    }
}