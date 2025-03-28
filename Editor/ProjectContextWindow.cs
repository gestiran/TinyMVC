﻿using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TinyMVC.Boot;
using UnityEditor;

namespace TinyMVC.Editor {
    internal class ProjectContextWindow : OdinEditorWindow {
        [ShowInInspector, HideReferenceObjectPicker, HideDuplicateReferenceBox, ShowIf("@_data != null && EditorApplication.isPlaying")]
        private static ProjectData _data;
        
        [ShowInInspector, ReadOnly, HideLabel, HideInPlayMode]
        private string _label = "Active only in PlayMode";
        
        public ProjectContextWindow() => EditorApplication.playModeStateChanged += StateChange;
        
        [MenuItem("Window/TinyMVC/ProjectContext", priority = 0)]
        private static void OpenWindow() {
            ProjectContextWindow window = GetWindow<ProjectContextWindow>("ProjectContext");
            window.Show();
        }
        
        private async void StateChange(PlayModeStateChange state) {
            if (state != PlayModeStateChange.EnteredPlayMode) {
                return;
            }
            
            await Task.Yield();
            
            _data = ProjectContext.data;
        }
        
        [Button("Reset"), HideInEditorMode, ShowIf("@_data == null && EditorApplication.isPlaying")]
        private void Reset() => _data = ProjectContext.data;
    }
}