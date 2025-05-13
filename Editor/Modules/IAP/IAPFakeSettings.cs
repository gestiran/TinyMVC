using System.Collections.Generic;
using UnityEditor;

namespace TinyMVC.Editor.Modules.IAP {
    [InitializeOnLoad]
    public static class IAPFakeSettings {
        public static bool isEnable { get; private set; }
        
        private const string _DEFINE = "UNITY_PURCHASING_FAKE";
        
        static IAPFakeSettings() => LoadStartState();
        
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider() {
            SettingsProvider provider = new SettingsProvider("Project/Player/Fake IAP", SettingsScope.Project);
            
            provider.label = "Fake IAP";
            provider.guiHandler = OnDrawSettings;
            provider.keywords = new HashSet<string>(new[] { "IAP", "Purchase" });
            
            LoadStartState();
            
            return provider;
        }
        
        public static void Enable() {
            if (isEnable) {
                return;
            }
            
            isEnable = true;
            EnableDefine(_DEFINE, BuildTargetGroup.Android);
        }
        
        public static void Disable() {
            if (isEnable == false) {
                return;
            }
            
            isEnable = false;
            DisableDefine(_DEFINE, BuildTargetGroup.Android);
        }
        
        private static void LoadStartState() => isEnable = CalculateEnable(_DEFINE, BuildTargetGroup.Android);
        
        private static void OnDrawSettings(string _) {
            bool value = EditorGUILayout.Toggle("Is Enable", isEnable);
            
            if (value == isEnable) {
                return;
            }
            
            if (value) {
                Enable();
            } else {
                Disable();
            }
        }
        
        private static void EnableDefine(string define, BuildTargetGroup targetGroup) {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            
            if (defines.Contains(define)) {
                return;
            }
            
            defines = $"{defines};{define}";
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }
        
        private static void DisableDefine(string define, BuildTargetGroup targetGroup) {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            
            if (defines.Contains(define) == false) {
                return;
            }
            
            defines = defines.Replace(define, "").Replace(";;", ";");
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }
        
        private static bool CalculateEnable(string define, BuildTargetGroup targetGroup) {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Contains(define);
        }
    }
}