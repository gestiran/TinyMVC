using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TinyMVC.Parameters {
    public sealed class TinyMVCParameters : ScriptableObject {
        [field: SerializeField, HideInInspector]
        internal bool isEnableAutoReload { get; private set; }
        
        private const string _PATH = "TinyMVCParameters";
        
        public static TinyMVCParameters LoadFromResources() {
            TinyMVCParameters result = Resources.Load<TinyMVCParameters>(_PATH);
            
        #if UNITY_EDITOR
            if (result == null) {
                result = CreateInstance<TinyMVCParameters>();
                AssetDatabase.CreateAsset(result, $"Assets/Resources/{_PATH}.asset");
            }
        #endif
            
            return result;
        }
        
    #if UNITY_EDITOR
        internal void ChangeAutoReload(bool value) {
            isEnableAutoReload = value;
            EditorUtility.SetDirty(this);
        }
    #endif
    }
}