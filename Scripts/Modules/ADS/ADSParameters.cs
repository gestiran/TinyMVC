using System;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_NUGET_NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

namespace TinyMVC.Modules.ADS {
    [CreateAssetMenu(fileName = nameof(ADSParameters), menuName = "API/" + nameof(ADSParameters))]
    public sealed class ADSParameters : ScriptableObject {
        [field: SerializeField, BoxGroup("Debug")]
        public bool fullNoADSMode { get; private set; }
        
        [field: SerializeField, BoxGroup("Remote")]
        public RemoteConfig remoteConfig { get; private set; }
        
        [field: SerializeField, BoxGroup("Tokens")]
        public int initialRewardTokensCount { get; private set; } = 0;
        
        [field: SerializeField, FoldoutGroup("IDs"), BoxGroup("IDs/Android")]
        public Config android { get; private set; }
        
        [field: SerializeField, BoxGroup("IDs/IOS")]
        public Config ios { get; private set; }
        
        public const byte TARGET_AGE = 16;
        
        [Serializable, HideLabel, InlineProperty]
        public sealed class RemoteConfig {
            [JsonIgnore] public int beforeFirstInterstitial => _beforeFirstInterstitial;
            [JsonIgnore] public int beforeAppStartInterstitial => _beforeAppStartInterstitial;
            [JsonIgnore] public int rewardInterstitialDisable => _rewardInterstitialDisable;
            [JsonIgnore] public int tokensPurchaseInterstitialDisable => _tokensPurchaseInterstitialDisable;
            [JsonIgnore] public int bannerUpdateTime => _bannerUpdateTime;
            [JsonIgnore] public int bannerRewardsLimit => _bannerRewardsLimit;
            
            [SerializeField, JsonProperty("beforeFirstInterstitial"), BoxGroup("Interstitial"), OnValueChanged("UpdateRemote")]
            private int _beforeFirstInterstitial;
            
            [SerializeField, JsonProperty("beforeAppStartInterstitial"), BoxGroup("Interstitial"), OnValueChanged("UpdateRemote")]
            private int _beforeAppStartInterstitial;
            
            [SerializeField, JsonProperty("rewardInterstitialDisable"), BoxGroup("Interstitial"), OnValueChanged("UpdateRemote")]
            private int _rewardInterstitialDisable;
            
            [SerializeField, JsonProperty("tokensPurchaseInterstitialDisable"), BoxGroup("Interstitial"), OnValueChanged("UpdateRemote")]
            private int _tokensPurchaseInterstitialDisable;
            
            [SerializeField, JsonProperty("bannerUpdateTime"), BoxGroup("Banner"), OnValueChanged("UpdateRemote")]
            private int _bannerUpdateTime;
            
            [SerializeField, JsonProperty("bannerRewardsLimit"), BoxGroup("Banner"), OnValueChanged("UpdateRemote")]
            private int _bannerRewardsLimit;
            
        #if UNITY_EDITOR
            [ShowInInspector, JsonIgnore, ReadOnly]
            private string _remote;
        #endif
            
            public RemoteConfig() {
                try {
                    RemoteConfig config = LoadFromResources().remoteConfig;
                    
                    _beforeFirstInterstitial = config._beforeFirstInterstitial;
                    _beforeAppStartInterstitial = config._beforeAppStartInterstitial;
                    _rewardInterstitialDisable = config._rewardInterstitialDisable;
                    _tokensPurchaseInterstitialDisable = config._tokensPurchaseInterstitialDisable;
                    _bannerUpdateTime = config._bannerUpdateTime;
                    _bannerRewardsLimit = config._bannerRewardsLimit;
                } catch (Exception) {
                    // ignored
                }
            }
            
        #if UNITY_EDITOR
            [OnInspectorInit]
            private void UpdateRemote() => _remote = JsonConvert.SerializeObject(this);
        #endif
        }
        
        [Serializable, HideLabel, InlineProperty]
        public sealed class Config {
            [field: SerializeField, BoxGroup("Kids")]
            public AgeGroup kids { get; private set; }
            
            [field: SerializeField, BoxGroup("General")]
            public AgeGroup general { get; private set; }
            
            [Serializable, HideLabel, InlineProperty]
            public sealed class AgeGroup {
                [field: SerializeField]
                public string banner { get; private set; }
                
                [field: SerializeField]
                public string interstitial { get; private set; }
                
                [field: SerializeField]
                public string reward { get; private set; }
            }
        }
        
        private const string _PATH = "Application/" + nameof(ADSParameters);
        
        public static ADSParameters LoadFromResources() => Resources.Load<ADSParameters>(_PATH);
        
        public void SetRemoteData(RemoteConfig data) => remoteConfig = data;
    }
}