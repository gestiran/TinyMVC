using System;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR && I2_LOCALIZE && UNITY_EDITOR
using I2.Loc;
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Modules.Push {
    [CreateAssetMenu(fileName = nameof(PushParameters), menuName = "API/" + nameof(PushParameters))]
    public sealed class PushParameters : ScriptableObject {
        [field: SerializeField]
        public string appTitle { get; private set; } = "UnityApplication";
        
        [SerializeField]
        private NotificationData[] _notifications;
        
        [Serializable]
        public sealed class NotificationData {
            [field: SerializeField]
            internal string key { get; private set; }
            
            [field: SerializeField
                    #if ODIN_INSPECTOR && I2_LOCALIZE && UNITY_EDITOR
                  , ValueDropdown(nameof(GetAllTerms))
                #endif
            ]
            public string term { get; private set; }
            
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