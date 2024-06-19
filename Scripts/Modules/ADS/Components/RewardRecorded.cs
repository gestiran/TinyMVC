using UnityEngine;

namespace TinyMVC.Modules.ADS.Components {
    [DisallowMultipleComponent]
    public sealed class RewardRecorded : MonoBehaviour {
        [field: SerializeField]
        public State state { get; private set; }
        
        public enum State : byte {
            Nothing,
            Show,
            Hide
        }
        
        #if GOOGLE_ADS_MOBILE
        
        private void Start() {
            switch (state) {
                case State.Show:
                    ShowOnActive(API<ADSModule>.module.IsLoadRewarded());
                    API<ADSModule>.module.onRewardActiveStateChange += ShowOnActive;
                    break;
                
                case State.Hide:
                    HideOnActive(API<ADSModule>.module.IsLoadRewarded());
                    API<ADSModule>.module.onRewardActiveStateChange += HideOnActive;
                    break;
            }
        }
        
        private void OnDestroy() {
            switch (state) {
                case State.Show:
                    API<ADSModule>.module.onRewardActiveStateChange -= ShowOnActive;
                    break;
                
                case State.Hide:
                    API<ADSModule>.module.onRewardActiveStateChange -= HideOnActive;
                    break;
            }
        }
        
        private void ShowOnActive(bool isActive) {
            if (isActive) {
                Show();
            } else {
                Hide();
            }
        }
        
        private void HideOnActive(bool isActive) {
            if (isActive) {
                Hide();
            } else {
                Show();
            }
        }
        
        private void Show() => gameObject.SetActive(true);
        
        private void Hide() => gameObject.SetActive(false);
        
        #endif
    }
}