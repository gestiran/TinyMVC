// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if UNITY_NUGET_NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

namespace TinyMVC.Modules.ADS {
    [CreateAssetMenu(fileName = nameof(ADSParameters), menuName = "API/" + nameof(ADSParameters))]
    public sealed class ADSParameters : ScriptableObject {
    #if ODIN_INSPECTOR
        [field: BoxGroup("Debug")]
    #endif
        [field: SerializeField]
        public bool fullNoADSMode { get; private set; }
        
    #if ODIN_INSPECTOR
        [field: BoxGroup("Remote")]
    #endif
        [field: SerializeField]
        public RemoteConfig remoteConfig { get; private set; } = RemoteConfig.Empty();
        
    #if ODIN_INSPECTOR
        [field: BoxGroup("Tokens")]
    #endif
        [field: SerializeField]
        public int initialRewardTokensCount { get; private set; } = 0;
        
    #if ODIN_INSPECTOR
        [field: FoldoutGroup("IDs"), BoxGroup("IDs/Android")]
    #endif
        [field: SerializeField]
        public Config android { get; private set; }
        
    #if ODIN_INSPECTOR
        [field: BoxGroup("IDs/IOS")]
    #endif
        [field: SerializeField]
        public Config ios { get; private set; }
        
        public const byte TARGET_AGE = 16;
        
    #if ODIN_INSPECTOR
        [field: HideLabel, InlineProperty]
    #endif
        [Serializable]
        public sealed class RemoteConfig {
            [JsonIgnore] public int beforeFirstInterstitial => _beforeFirstInterstitial;
            [JsonIgnore] public int beforeAppStartInterstitial => _beforeAppStartInterstitial;
            [JsonIgnore] public int rewardInterstitialDisable => _rewardInterstitialDisable;
            [JsonIgnore] public int tokensPurchaseInterstitialDisable => _tokensPurchaseInterstitialDisable;
            [JsonIgnore] public int bannerUpdateTime => _bannerUpdateTime;
            [JsonIgnore] public int bannerRewardsLimit => _bannerRewardsLimit;
            
        #if ODIN_INSPECTOR
            [BoxGroup("Interstitial"), OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("beforeFirstInterstitial")]
            private int _beforeFirstInterstitial;
            
        #if ODIN_INSPECTOR
            [BoxGroup("Interstitial"), OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("beforeAppStartInterstitial")]
            private int _beforeAppStartInterstitial;
            
        #if ODIN_INSPECTOR
            [BoxGroup("Interstitial"), OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("rewardInterstitialDisable")]
            private int _rewardInterstitialDisable;
            
        #if ODIN_INSPECTOR
            [BoxGroup("Interstitial"), OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("tokensPurchaseInterstitialDisable")]
            private int _tokensPurchaseInterstitialDisable;
            
        #if ODIN_INSPECTOR
            [BoxGroup("Banner"), OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("bannerUpdateTime")]
            private int _bannerUpdateTime;
            
        #if ODIN_INSPECTOR
            [BoxGroup("Banner"), OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("bannerRewardsLimit")]
            private int _bannerRewardsLimit;
            
        #if UNITY_EDITOR && ODIN_INSPECTOR
            [JsonIgnore, ShowInInspector, ReadOnly]
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
            
            private RemoteConfig(int _) { }
            
            public static RemoteConfig Empty() => new RemoteConfig(0); 
            
        #if UNITY_EDITOR && ODIN_INSPECTOR
            [OnInspectorInit]
            private void UpdateRemote() => _remote = JsonConvert.SerializeObject(this);
        #endif
        }
        
    #if ODIN_INSPECTOR
        [HideLabel, InlineProperty]
    #endif
        [Serializable]
        public sealed class Config {
        #if ODIN_INSPECTOR
            [field: BoxGroup("Kids")]
        #endif
            [field: SerializeField]
            public AgeGroup kids { get; private set; }
            
        #if ODIN_INSPECTOR
            [field: BoxGroup("General")]
        #endif
            [field: SerializeField]
            public AgeGroup general { get; private set; }
            
        #if ODIN_INSPECTOR
            [HideLabel, InlineProperty]
        #endif
            [Serializable]
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
        
        public static ADSParameters LoadFromResources() {
            ADSParameters parameters = Resources.Load<ADSParameters>(_PATH);
            
            if (parameters != null) {
                return parameters;
            }
            
            return Resources.Load<ADSParameters>($"{_PATH}Default");
        }
        
        public void SetRemoteData(RemoteConfig data) => remoteConfig = data;
    }
}