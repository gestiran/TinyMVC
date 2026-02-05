// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Parameters;
using TinyUtilities.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace TinyMVC.Editor.Boot.AutoGenerate {
    internal sealed class AutoGenerateModule {
        private TinyMVCParameters _parameters;
        
        public AutoGenerateModule() {
            Init();
        }
        
        public void Init() {
            _parameters = TinyMVCParameters.LoadFromResources();
        }
        
        public void Draw() {
            EditorGUILayout.LabelField($"SceneContext auto generate ({(_parameters.isEnableAutoReload ? "Enabled" : "Disabled")})", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Auto use \"Generate\" on SceneContext after PlayModeStart\nThis is EditorOnly function!", MessageType.Info);
            
            GUIContent isEnableLabel = new GUIContent("Enabled");
            isEnableLabel.tooltip = "Enable SceneContext auto generate.";
            GUIDrawUtility.DrawToggle(isEnableLabel, _parameters.isEnableAutoReload, _parameters.ChangeAutoReload);
        }
    }
}