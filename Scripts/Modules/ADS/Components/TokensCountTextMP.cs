using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace TinyMVC.Modules.ADS.Components {
    [RequireComponent(typeof(TMP_Text))]
    public sealed class TokensCountTextMP : TokensCount {
        [SerializeField, Required]
        private TMP_Text _thisText;
        
        protected override void UpdateCount(int count) => _thisText.text = $"{count}";
        
    #if UNITY_EDITOR
        
        [ContextMenu("Soft Reset")]
        private void Reset() {
            _thisText = GetComponent<TMP_Text>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
    #endif
    }
}