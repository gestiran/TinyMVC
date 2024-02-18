#if ODIN_INSPECTOR
    using System.Threading.Tasks;
    using TinyMVC.Boot;
    using UnityEditor;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;

    namespace TinyMVC {
        internal class ProjectContextWindow : OdinEditorWindow {
            [ShowInInspector, HideReferenceObjectPicker, HideDuplicateReferenceBox, ShowIf("@_context != null && EditorApplication.isPlaying")]
            private static ProjectContext _context;

            [ShowInInspector, ReadOnly, HideLabel, HideInPlayMode]
            private string _label = "Active only in PlayMode";
            
            public ProjectContextWindow() => EditorApplication.playModeStateChanged += StateChange;

            [MenuItem("Window/ProjectContext", priority = 0)]
            private static void OpenWindow() {
                ProjectContextWindow window = GetWindow<ProjectContextWindow>("ProjectContext");
                window.Show();
            }

            private async void StateChange(PlayModeStateChange state) {
                if (state != PlayModeStateChange.EnteredPlayMode) {
                    return;
                }
                
                await Task.Yield();

                _context = ProjectContext.current;
            }

            [Button("Reset"), HideInEditorMode, ShowIf("@_context == null && EditorApplication.isPlaying")]
            private void Reset() => _context = ProjectContext.current;
        }
    }

#endif