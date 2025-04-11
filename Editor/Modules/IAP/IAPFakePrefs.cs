using UnityEditor;

namespace TinyMVC.Editor.Modules.IAP {
    public sealed class IAPFakePrefs {
        private readonly string _project;
        
        private const string _IS_FAKE_IAP = "EditorFakeIAP";
        
        public IAPFakePrefs(string project) => _project = project;
        
        public void SaveActiveState(bool value) {
            EditorPrefs.SetBool($"{_IS_FAKE_IAP}_{_project}", value);
        }
        
        public bool LoadActiveState() {
            return EditorPrefs.GetBool($"{_IS_FAKE_IAP}_{_project}", false);
        }
    }
}