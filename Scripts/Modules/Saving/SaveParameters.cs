using UnityEngine;

namespace TinyMVC.Modules.Saving {
    [CreateAssetMenu(fileName = nameof(SaveParameters), menuName = "API/" + nameof(SaveParameters))]
    public sealed class SaveParameters : ScriptableObject {
        [field: SerializeField]
        public string rootDirectory { get; private set; } = ROOT_DIRECTORY;
        
        [field: SerializeField]
        public string versionLabel { get; private set; } = VERSION_LABEL;
        
        public const string ROOT_DIRECTORY = "UserData";
        public const string VERSION_LABEL = "V_01";
        
        private const string _PATH = nameof(SaveParameters);
        
        public static SaveParameters LoadFromResources() => Resources.Load<SaveParameters>(_PATH);
        
    #if UNITY_EDITOR
        
        public static SaveParameters LoadFromAssets() => UnityEditor.AssetDatabase.LoadAssetAtPath<SaveParameters>(System.IO.Path.Combine($"Assets/Resources/{_PATH}"));
        
    #endif
    }
}