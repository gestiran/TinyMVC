using System;
using UnityEngine;

#if UNITY_EDITOR && UNITY_NUGET_NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

namespace TinyMVC.Modules.RateUs {
    [CreateAssetMenu(fileName = nameof(RateUsParameters), menuName = "API/" + nameof(RateUsParameters))]
    public sealed class RateUsParameters : ScriptableObject {
        [field: SerializeField]
        public bool isEnableRateUs { get; private set; } = true;
        
        [field: SerializeField, Header("Timers:")]
        public int firstShowDelay { get; private set; } = 20;
        
        [field: SerializeField]
        public int otherShowDelay { get; private set; } = 60;
        
        [field: SerializeField]
        public int afterAppStartDelay { get; private set; } = 10;
        
        [field: SerializeField]
        public int playerDeadDelay { get; private set; } = 3;
        
        [field: SerializeField]
        public int eventFailedDelay { get; private set; } = 3;
        
        [field: SerializeField]
        public int interstitialShowDelay { get; private set; } = 3;
        
        private const string _PATH = "Application/" + nameof(RateUsParameters);
        
        #if UNITY_EDITOR
        
        [SerializeField, Header("Remote:")]
        private string _remote;
        
        #endif
        
        [Serializable]
        public sealed class Remotes {
            [SerializeField]
            public int firstShowDelay; 
            
            [SerializeField]
            public int otherShowDelay; 
            
            [SerializeField]
            public int afterAppStartDelay; 
            
            [SerializeField]
            public int playerDeadDelay;
            
            [SerializeField]
            public int eventFailedDelay;
            
            [SerializeField]
            public int interstitialShowDelay;
        }
        
        public static RateUsParameters LoadFromResources() => Resources.Load<RateUsParameters>(_PATH);
        
        public void SetRemoteData(Remotes data) {
            firstShowDelay = data.firstShowDelay;
            otherShowDelay = data.otherShowDelay;
            afterAppStartDelay = data.afterAppStartDelay;
            playerDeadDelay = data.playerDeadDelay;
            eventFailedDelay = data.eventFailedDelay;
            interstitialShowDelay = data.interstitialShowDelay;
        }
        
        public Remotes DefaultRemotes() {
            Remotes data = new Remotes();
            
            data.firstShowDelay = firstShowDelay;
            data.otherShowDelay = otherShowDelay;
            data.afterAppStartDelay = afterAppStartDelay;
            data.playerDeadDelay = playerDeadDelay;
            data.eventFailedDelay = eventFailedDelay;
            data.interstitialShowDelay = interstitialShowDelay;
            
            return data;
        }
        
        #if UNITY_EDITOR && UNITY_NUGET_NEWTONSOFT_JSON
        
        private void OnValidate() => _remote = JsonConvert.SerializeObject(DefaultRemotes());
        
        #endif
    }
}