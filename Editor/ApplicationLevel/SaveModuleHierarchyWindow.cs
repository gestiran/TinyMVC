using TinyMVC.ApplicationLevel.Saving;
using UnityEditor;
using UnityEngine.UIElements;

namespace TinyMVC.ApplicationLevel {
    internal sealed class SaveModuleHierarchyWindow : EditorWindow {
        private Foldout _group;
        
        private StyleSheet _style;

        private const string _STYLE_PATH = "Packages/com.ges.tinymvc/Style/ApplicationLevel/SaveModuleHierarchyStyle.uss";
        
        [MenuItem("Window/TinyMVC/SaveModuleHierarchy", priority = 0)]
        private static void OpenWindow() => GetWindow<SaveModuleHierarchyWindow>("SaveModuleHierarchy");

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
            API<SaveModule>.module.GetHierarchy_Editor(_group.contentContainer);
        }
    }
}