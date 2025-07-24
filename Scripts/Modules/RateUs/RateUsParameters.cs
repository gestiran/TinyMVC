using System;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if UNITY_NUGET_NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

namespace TinyMVC.Modules.RateUs {
    [CreateAssetMenu(fileName = nameof(RateUsParameters), menuName = "API/" + nameof(RateUsParameters))]
    public sealed class RateUsParameters : ScriptableObject {
        [field: SerializeField]
        public bool isEnableRateUs { get; private set; } = true;
        
    #if ODIN_INSPECTOR
        [field: BoxGroup("Remote")]
    #endif
        [field: SerializeField]
        public RemoteConfig remoteConfig { get; private set; } = RemoteConfig.Empty();
        
        private const string _PATH = "Application/" + nameof(RateUsParameters);
    #if ODIN_INSPECTOR
        [HideLabel, InlineProperty]
    #endif
        [Serializable]
        public sealed class RemoteConfig {
            [JsonIgnore] public int firstShowDelay => _firstShowDelay;
            [JsonIgnore] public int otherShowDelay => _otherShowDelay;
            [JsonIgnore] public int afterAppStartDelay => _afterAppStartDelay;
            [JsonIgnore] public int playerDeadDelay => _playerDeadDelay;
            [JsonIgnore] public int eventFailedDelay => _eventFailedDelay;
            [JsonIgnore] public int interstitialShowDelay => _interstitialShowDelay;
            
        #if ODIN_INSPECTOR
            [OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("firstShowDelay")]
            private int _firstShowDelay;
            
        #if ODIN_INSPECTOR
            [OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("otherShowDelay")]
            private int _otherShowDelay;
            
        #if ODIN_INSPECTOR
            [OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("afterAppStartDelay")]
            private int _afterAppStartDelay;
            
        #if ODIN_INSPECTOR
            [OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("playerDeadDelay")]
            private int _playerDeadDelay;
            
        #if ODIN_INSPECTOR
            [OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("eventFailedDelay")]
            private int _eventFailedDelay;
            
        #if ODIN_INSPECTOR
            [OnValueChanged("UpdateRemote")]
        #endif
            [SerializeField, JsonProperty("interstitialShowDelay")]
            private int _interstitialShowDelay;
            
        #if UNITY_EDITOR && ODIN_INSPECTOR
            [JsonIgnore, ShowInInspector, ReadOnly]
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
            
            private RemoteConfig(int _) { }
            
            public static RemoteConfig Empty() => new RemoteConfig(0);
            
        #if UNITY_EDITOR && ODIN_INSPECTOR
            [OnInspectorInit]
            private void UpdateRemote() => _remote = JsonConvert.SerializeObject(this);
        #endif
        }
        
        public static RateUsParameters LoadFromResources() {
            RateUsParameters parameters = Resources.Load<RateUsParameters>(_PATH);
            
            if (parameters != null) {
                return parameters;
            }
            
            return Resources.Load<RateUsParameters>($"{_PATH}Default");
        }
        
        public void SetRemoteData(RemoteConfig config) => remoteConfig = config;
    }
}