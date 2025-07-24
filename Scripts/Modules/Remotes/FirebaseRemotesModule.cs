// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
using System;
using Firebase.RemoteConfig;
#endif

#if UNITY_NUGET_NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

namespace TinyMVC.Modules.Remotes {
    public abstract class FirebaseRemotesModule : IApplicationModule {
        protected abstract bool _isAlwaysDefault { get; }
        
    #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
        public LastFetchStatus lastFetchStatus { get; private set; }
        
        private static FirebaseRemoteConfig _instance;
    #endif
        
        protected readonly Dictionary<string, object> _defaultValues;
        
        protected FirebaseRemotesModule() => _defaultValues = new Dictionary<string, object>();
        
        public async Task Initialize() {
            LoadResources();
            FillDefaultValues(_defaultValues);
            
        #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
            
            lastFetchStatus = LastFetchStatus.Pending;
            _instance = FirebaseRemoteConfig.DefaultInstance;
            
            await _instance.SetDefaultsAsync(_defaultValues);
            
            try {
                await _instance.FetchAsync();
            } catch (Exception exception) {
                Debug.LogWarning(exception);
                lastFetchStatus = LastFetchStatus.Failure;
                ApplyRemotes();
                return;
            }
            
            lastFetchStatus = _instance.Info.LastFetchStatus;
            
            if (lastFetchStatus == LastFetchStatus.Success) {
                Debug.Log($"RemotesModule: {lastFetchStatus}");
                await _instance.ActivateAsync();
            } else if (lastFetchStatus == LastFetchStatus.Pending) {
                Debug.LogWarning($"RemotesModule: {lastFetchStatus}");
            } else {
                Debug.LogError($"RemotesModule: {lastFetchStatus}");
            }
            
            ApplyRemotes();
            
        #endif
        }
        
        public string GetString(string key) {
        #if GOOGLE_FIREBASE_APP && GOOGLE_FIREBASE_REMOTE_CONFIGS
            
            if (_isAlwaysDefault == false && lastFetchStatus == LastFetchStatus.Success) {
                try {
                    string stringValue = _instance.GetValue(key).StringValue;
                    
                    if (string.IsNullOrEmpty(stringValue) == false) {
                        return _instance.GetValue(key).StringValue;    
                    }
                } catch (Exception exception) {
                    Debug.LogWarning(exception);
                }
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
            if (_isAlwaysDefault == false && lastFetchStatus == LastFetchStatus.Success) {
                try {
                #if UNITY_NUGET_NEWTONSOFT_JSON
                    string stringValue = _instance.GetValue(key).StringValue;
                    
                    if (string.IsNullOrEmpty(stringValue) == false) {
                        return JsonConvert.DeserializeObject<T>(stringValue);    
                    }
                #else
                    Debug.LogError($"Can't convert remote data with key {key}, please use unity newtonsoft JSON!");
                #endif
                } catch (Exception exception) {
                    Debug.LogWarning(exception);
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
            if (_isAlwaysDefault == false && lastFetchStatus == LastFetchStatus.Success) {
                try {
                    if (float.TryParse($"{_instance.GetValue(key).DoubleValue}", out float remoteValue)) {
                        return remoteValue;
                    }
                } catch (Exception exception) {
                    Debug.LogWarning(exception);
                }
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
            if (_isAlwaysDefault == false && lastFetchStatus == LastFetchStatus.Success) {
                try {
                    if (int.TryParse($"{_instance.GetValue(key).LongValue}", out int remoteValue)) {
                        return remoteValue;
                    }
                } catch (Exception exception) {
                    Debug.LogWarning(exception);
                }
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
            if (_isAlwaysDefault == false && lastFetchStatus == LastFetchStatus.Success) {
                try {
                    return _instance.GetValue(key).BooleanValue;
                } catch (Exception exception) {
                    Debug.LogWarning(exception);
                }
            }
        #endif
            
            if (_defaultValues.TryGetValue(key, out object value)) {
                return (bool)value;
            }
            
            Debug.LogError($"Can't find remote value with key {key}!");
            
            return false;
        }
        
        protected abstract void LoadResources();
        
        protected abstract void FillDefaultValues(Dictionary<string, object> defaults);
        
        protected abstract void ApplyRemotes();
    }
}