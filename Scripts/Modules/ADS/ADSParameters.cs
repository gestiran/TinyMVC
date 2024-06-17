using System;
using UnityEngine;

#if UNITY_EDITOR && UNITY_NUGET_NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

namespace TinyMVC.Modules.ADS {
    [CreateAssetMenu(fileName = nameof(ADSParameters), menuName = "API/" + nameof(ADSParameters))]
    public sealed class ADSParameters : ScriptableObject {
        [field: SerializeField, Header("Debug:")]
        public bool fullNoADSMode { get; private set; }
        
        [field: SerializeField, Header("Interstitial:")]
        public int beforeFirstInterstitial { get; private set; } = 30;
        
        [field: SerializeField]
        public int beforeAppStartInterstitial { get; private set; } = 3;
        
        [field: SerializeField]
        public int rewardInterstitialDisable { get; private set; } = 10;
        
        [field: SerializeField, Header("Banner:")]
        public int bannerUpdateTime { get; private set; } = 60;
        
        [field: SerializeField]
        public int bannerRewardsLimit { get; private set; } = 4;
        
        [field: SerializeField, Header("Ids:")]
        public Config android { get; private set; }
        
        [field: SerializeField]
        public Config ios { get; private set; }
        
        #if UNITY_EDITOR
        
        [SerializeField, Header("Remote:")]
        private string _remote;
        
        #endif
        
        public const byte TARGET_AGE = 16;
        
        [Serializable]
        public sealed class Remotes {
            [SerializeField]
            public int beforeFirstInterstitial;
            
            [SerializeField]
            public int beforeAppStartInterstitial;
            
            [SerializeField]
            public int rewardInterstitialDisable;
            
            [SerializeField]
            public int bannerUpdateTime;
            
            [SerializeField]
            public int bannerRewardsLimit;
        }
        
        [Serializable]
        public sealed class Config {
            [field: SerializeField]
            public AgeGroup kids { get; private set; }
            
            [field: SerializeField]
            public AgeGroup general { get; private set; }
            
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
        
        public static ADSParameters LoadFromResources() => Resources.Load<ADSParameters>(_PATH);
        
        public void SetRemoteData(Remotes data) {
            beforeFirstInterstitial = data.beforeFirstInterstitial;
            beforeAppStartInterstitial = data.beforeAppStartInterstitial;
            rewardInterstitialDisable = data.rewardInterstitialDisable;
            bannerUpdateTime = data.bannerUpdateTime;
            bannerRewardsLimit = data.bannerRewardsLimit;
        }
        
        public Remotes DefaultRemotes() {
            Remotes data = new Remotes();
            
            data.beforeFirstInterstitial = beforeFirstInterstitial;
            data.beforeAppStartInterstitial = beforeAppStartInterstitial;
            data.rewardInterstitialDisable = rewardInterstitialDisable;
            data.bannerUpdateTime = bannerUpdateTime;
            data.bannerRewardsLimit = bannerRewardsLimit;
            
            return data;
        }
        
        #if UNITY_EDITOR && UNITY_NUGET_NEWTONSOFT_JSON
        
        private void OnValidate() => _remote = JsonConvert.SerializeObject(DefaultRemotes());
        
        #endif
    }
}