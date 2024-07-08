using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
using System;
using Firebase.Extensions;
using Firebase.RemoteConfig;
#endif

#if UNITY_NUGET_NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

namespace TinyMVC.Modules.Remotes {
    public abstract class FirebaseRemotesModule : IApplicationModule {
        #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
        public LastFetchStatus lastFetchStatus { get; private set; }
        
        private readonly FirebaseRemoteConfig _instance;
        #endif
        
        private readonly Dictionary<string, object> _defaultValues;
        
        #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
        private const int _MAX_DELAY = 3000;
        private const int _STEP = 100;
        #endif
        
        protected FirebaseRemotesModule() {
            _defaultValues = CreateDefaultValues();
            
            #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
            lastFetchStatus = LastFetchStatus.Pending;
            _instance = FirebaseRemoteConfig.DefaultInstance;
            _instance.SetDefaultsAsync(_defaultValues).ContinueWithOnMainThread(StartFetch);
            #endif
        }
        
        public async Task WaitInitialization() {
            #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
            int time = _MAX_DELAY;
            
            while (time > 0) {
                if (lastFetchStatus != LastFetchStatus.Pending) {
                    return;
                }
                
                await Task.Delay(_STEP);
                time -= _STEP;
            }
            #endif
            await Task.Yield();
        }
        
        public string GetString(string key) {
            #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
            if (lastFetchStatus == LastFetchStatus.Success) {
                return _instance.GetValue(key).StringValue;
            }
            #endif
            
            if (_defaultValues.TryGetValue(key, out object value)) {
                return (string)value;
            }
            
            Debug.LogError($"Can't find remote value with key {key}!");
            
            return null;
        }
        
        
        public T GetValue<T>(string key) {
            #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
            if (lastFetchStatus == LastFetchStatus.Success) {
                try {
                    #if UNITY_NUGET_NEWTONSOFT_JSON
                    return JsonConvert.DeserializeObject<T>(_instance.GetValue(key).StringValue);
                    #else
                    Debug.LogError($"Can't convert remote data with key {key}, please use unity newtonsoft JSON!");
                    
                    return default;
                    
                    #endif
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
            #endif
            
            if (_defaultValues.TryGetValue(key, out object value)) {
                #if UNITY_NUGET_NEWTONSOFT_JSON
                return JsonConvert.DeserializeObject<T>((string)value);
                #else
                if (value is T result) {
                    return result;
                }
                
                Debug.LogError($"Can't convert remote data with key {key}, please use unity newtonsoft JSON!");
                
                return default;
                #endif
            }
            
            Debug.LogError($"Can't find remote value with key {key}!");
            
            return default;
        }
        
        public float GetFloat(string key) {
            #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
            if (lastFetchStatus == LastFetchStatus.Success && float.TryParse($"{_instance.GetValue(key).DoubleValue}", out float remoteValue)) {
                return remoteValue;
            }
            #endif
            
            if (_defaultValues.TryGetValue(key, out object value)) {
                return (float)value;
            }
            
            Debug.LogError($"Can't find remote value with key {key}!");
            
            return default;
        }
        
        public int GetInt(string key) {
            #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
            if (lastFetchStatus == LastFetchStatus.Success && int.TryParse($"{_instance.GetValue(key).LongValue}", out int remoteValue)) {
                return remoteValue;
            }
            #endif
            
            if (_defaultValues.TryGetValue(key, out object value)) {
                return (int)value;
            }
            
            Debug.LogError($"Can't find remote value with key {key}!");
            
            return default;
        }
        
        public bool GetBool(string key) {
            #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
            if (lastFetchStatus == LastFetchStatus.Success) {
                return _instance.GetValue(key).BooleanValue;
            }
            #endif
            
            if (_defaultValues.TryGetValue(key, out object value)) {
                return (bool)value;
            }
            
            Debug.LogError($"Can't find remote value with key {key}!");
            
            return false;
        }
        
        #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
        private void StartFetch(Task _) => _instance.FetchAsync().ContinueWithOnMainThread(OnFetchFinish);
        
        private void OnFetchFinish(Task _) {
            lastFetchStatus = _instance.Info.LastFetchStatus;
            
            if (lastFetchStatus == LastFetchStatus.Success) {
                Debug.Log($"RemotesModule: {lastFetchStatus}");
                
                _instance.ActivateAsync().ContinueWithOnMainThread(_ => ApplyRemotes());
            } else if (lastFetchStatus == LastFetchStatus.Pending) {
                Debug.LogWarning($"RemotesModule: {lastFetchStatus}");
            } else {
                Debug.LogError($"RemotesModule: {lastFetchStatus}");
            }
        }
        
        #endif
        
        protected abstract Dictionary<string, object> CreateDefaultValues();
        
        protected abstract void ApplyRemotes();
    }
}