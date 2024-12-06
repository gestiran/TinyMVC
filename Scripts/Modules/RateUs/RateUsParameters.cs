using System;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_NUGET_NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

namespace TinyMVC.Modules.RateUs {
    [CreateAssetMenu(fileName = nameof(RateUsParameters), menuName = "API/" + nameof(RateUsParameters))]
    public sealed class RateUsParameters : ScriptableObject {
        [field: SerializeField]
        public bool isEnableRateUs { get; private set; } = true;
        
        [field: SerializeField, BoxGroup("Remote")]
        public RemoteConfig remoteConfig { get; private set; }
        
        private const string _PATH = "Application/" + nameof(RateUsParameters);
        
        [Serializable, HideLabel, InlineProperty]
        public sealed class RemoteConfig {
            [JsonIgnore] public int firstShowDelay => _firstShowDelay;
            [JsonIgnore] public int otherShowDelay => _otherShowDelay;
            [JsonIgnore] public int afterAppStartDelay => _afterAppStartDelay;
            [JsonIgnore] public int playerDeadDelay => _playerDeadDelay;
            [JsonIgnore] public int eventFailedDelay => _eventFailedDelay;
            [JsonIgnore] public int interstitialShowDelay => _interstitialShowDelay;
            
            [SerializeField, JsonProperty("firstShowDelay"), OnValueChanged("UpdateRemote")]
            private int _firstShowDelay;
            
            [SerializeField, JsonProperty("otherShowDelay"), OnValueChanged("UpdateRemote")]
            private int _otherShowDelay;
            
            [SerializeField, JsonProperty("afterAppStartDelay"), OnValueChanged("UpdateRemote")]
            private int _afterAppStartDelay;
            
            [SerializeField, JsonProperty("playerDeadDelay"), OnValueChanged("UpdateRemote")]
            private int _playerDeadDelay;
            
            [SerializeField, JsonProperty("eventFailedDelay"), OnValueChanged("UpdateRemote")]
            private int _eventFailedDelay;
            
            [SerializeField, JsonProperty("interstitialShowDelay"), OnValueChanged("UpdateRemote")]
            private int _interstitialShowDelay;
            
        #if UNITY_EDITOR
            [ShowInInspector, JsonIgnore, ReadOnly]
            private string _remote;
        #endif
            
            public RemoteConfig() {
                try {
                    RemoteConfig config = LoadFromResources().remoteConfig;
                    
                    _firstShowDelay = config._firstShowDelay;
                    _otherShowDelay = config._otherShowDelay;
                    _afterAppStartDelay = config._afterAppStartDelay;
                    _playerDeadDelay = config._playerDeadDelay;
                    _eventFailedDelay = config._eventFailedDelay;
                    _interstitialShowDelay = config._interstitialShowDelay;
                } catch (Exception) {
                    // Ignore
                }
            }
            
        #if UNITY_EDITOR
            [OnInspectorInit]
            private void UpdateRemote() => _remote = JsonConvert.SerializeObject(this);
        #endif
        }
        
        public static RateUsParameters LoadFromResources() => Resources.Load<RateUsParameters>(_PATH);
        
        public void SetRemoteData(RemoteConfig config) => remoteConfig = config;
    }
}