// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using UnityEngine;

#if I2_LOCALIZE
using I2.Loc;
#endif

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
using System.Collections.Generic;
#endif

namespace TinyMVC.Modules.Push {
    [CreateAssetMenu(fileName = nameof(PushParameters), menuName = "API/" + nameof(PushParameters))]
    public sealed class PushParameters : ScriptableObject {
        [field: SerializeField]
        public string appTitle { get; private set; } = "UnityApplication";
        
        [SerializeReference]
        private NotificationData[] _notifications;
        
        [Serializable]
        public abstract class NotificationData {
            [field: SerializeField]
            internal string key { get; private set; }
            
            public abstract string GetText();
        }
        
        [Serializable]
        public sealed class NotificationTextData : NotificationData {
            [field: SerializeField]
            public string term { get; private set; }
            
            public override string GetText() => term;
        }
        
        [Serializable]
        public sealed class NotificationTermData : NotificationData {
        #if ODIN_INSPECTOR && I2_LOCALIZE && UNITY_EDITOR
            [field: ValueDropdown("GetAllTerms")]
        #endif
            [field: SerializeField]
            public string term { get; private set; }
            
            public override string GetText() {
            #if I2_LOCALIZE
                return LocalizationManager.GetTranslation(term);
            #endif
                return term;
            }
            
        #if ODIN_INSPECTOR && I2_LOCALIZE && UNITY_EDITOR
            private List<string> GetAllTerms() {
                if (TryGetSources(out LanguageSourceAsset source)) {
                    return source.mSource.GetTermsList();
                }
                
                return new List<string>();
            }
            
            private bool TryGetSources(out LanguageSourceAsset source) {
                source = Resources.Load<LanguageSourceAsset>("I2Languages");
                
                if (source == null) {
                    return false;
                }
                
                return true;
            }
            
            private List<string> GetAllIcons() {
                if (TryGetSources(out LanguageSourceAsset source)) {
                    return source.mSource.GetTermsList();
                }
                
                return new List<string>();
            }
        #endif
        }
        
        private const string _PATH = "Application/" + nameof(PushParameters);
        
        public static PushParameters LoadFromResources() => Resources.Load<PushParameters>(_PATH);
        
        public bool TryGetNotification(string key, out NotificationData notification) {
            for (int id = 0; id < _notifications.Length; id++) {
                if (key.Equals(_notifications[id].key)) {
                    notification = _notifications[id];
                    return true;
                }
            }
            
            notification = null;
            return false;
        }
    }
}