// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyMVC.Editor.Boot.AutoGenerate;
using UnityEditor;

namespace TinyMVC.Editor.Boot {
    [InitializeOnLoad]
    public static class ProjectContextSettings {
        private static readonly AutoGenerateModule _autoGenerate;
        
        static ProjectContextSettings() {
            _autoGenerate = new AutoGenerateModule();
            
            LoadStartState();
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider() {
            SettingsProvider provider = new SettingsProvider("Project/Editor/Project Context", SettingsScope.Project);
            
            provider.label = "Project Context";
            provider.guiHandler = OnDrawSettings;
            provider.keywords = new HashSet<string>(new[] { "Context", "Initialization" });
            
            LoadStartState();
            
            return provider;
        }
        
        private static void LoadStartState() {
            _autoGenerate.Init();
        }
        
        private static void OnDrawSettings(string obj) {
            EditorGUILayout.HelpBox("All settings auto saved in \"Resources/TinyMVCParameters\"", MessageType.Info);
            EditorGUILayout.Space();
            _autoGenerate.Draw();
        }
    }
}