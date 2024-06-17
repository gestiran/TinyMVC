using UnityEditor;
using UnityEngine.UIElements;

namespace TinyMVC.Editor.Modules.Saving {
    internal sealed class SaveModuleStatsWindow : EditorWindow {
        private Foldout _group;
        
        private StyleSheet _style;
        
        private const string _STYLE_PATH = "Packages/com.ges.tinymvc/Style/ApplicationLevel/SaveModuleHierarchyStyle.uss";
        
        [MenuItem("Window/TinyMVC/SaveModuleStats", priority = 0)]
        private static void OpenWindow() => GetWindow<SaveModuleStatsWindow>("SaveModuleStats");
        
        private void CreateGUI() {
            _style = AssetDatabase.LoadAssetAtPath<StyleSheet>(_STYLE_PATH);
            
            VisualElement root = rootVisualElement;
            rootVisualElement.styleSheets.Add(_style);
            
            ScrollView scroll = new ScrollView(ScrollViewMode.Vertical);
            
            _group = new Foldout();
            _group.text = "<b>Files</b>";
            _group.value = true;
            
            scroll.Add(_group);
            root.Add(scroll);
            
            Button button = new Button();
            button.name = "button";
            button.text = "Update Data";
            button.clicked += UpdateData;
            
            root.Add(button);
        }
        
        private void UpdateData() {
            _group.Clear();
            
            foreach (SaveModuleLog.StatsData data in SaveModuleLog.stats.Values) {
                _group.Add(new Label($"<b>{data.name}:</b> {data.size} :: {data.count}"));
            }
        }
    }
}