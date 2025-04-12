using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace TinyMVC.Editor.Modules.IAP {
    public sealed class IAPPreBuildWarning : IPreprocessBuildWithReport {
        public int callbackOrder => 0;
        
        public void OnPreprocessBuild(BuildReport report) {
            if (IAPFakeSettings.isEnable == false) {
                return;
            }
            
            bool proceed = EditorUtility.DisplayDialog("Fake IAP", "Fake IAP is active!", "Keep active", "Disable");
            
            if (proceed) {
                return;
            }
            
            IAPFakeSettings.Disable();
            
            //throw new BuildFailedException("Canceled by user due to active fake IAP");
        }
    }
}