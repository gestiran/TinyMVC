using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace TinyMVC.Editor.Modules.IAP {
    [InitializeOnLoad]
    public static class IAPFakeSettings {
        public static bool isEnable { get; private set; }
        
        private static readonly IAPFakePrefs _prefs;
        
        private const string _DEFINE = "UNITY_PURCHASING_FAKE";
        
        static IAPFakeSettings() {
            _prefs = new IAPFakePrefs(Path.GetFileName(Path.GetDirectoryName(Application.dataPath)));
            LoadStartState();
        }
        
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
            _prefs.SaveActiveState(isEnable);
            EnableDefine(_DEFINE, BuildTargetGroup.Android);
        }
        
        public static void Disable() {
            if (isEnable == false) {
                return;
            }
            
            isEnable = false;
            _prefs.SaveActiveState(isEnable);
            DisableDefine(_DEFINE, BuildTargetGroup.Android);
        }
        
        private static void LoadStartState() => isEnable = _prefs.LoadActiveState();
        
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnableDefine(string define, BuildTargetGroup targetGroup) {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            
            if (defines.Contains(define)) {
                return;
            }
            
            defines = $"{defines};{define}";
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DisableDefine(string define, BuildTargetGroup targetGroup) {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            
            if (defines.Contains(define) == false) {
                return;
            }
            
            defines = defines.Replace(define, "").Replace(";;", ";");
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }
    }
}