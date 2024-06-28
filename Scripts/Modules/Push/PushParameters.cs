using UnityEngine;

namespace TinyMVC.Modules.Push {
    [CreateAssetMenu(fileName = nameof(PushParameters), menuName = "API/" + nameof(PushParameters))]
    public sealed class PushParameters : ScriptableObject {
        [field: SerializeField]
        public string appTitle { get; private set; } = "UnityApplication";
        
        private const string _PATH = "Application/" + nameof(PushParameters);
        
        public static PushParameters LoadFromResources() => Resources.Load<PushParameters>(_PATH);
    }
}