// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TMPro;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Modules.ADS.Components {
    [RequireComponent(typeof(TMP_Text))]
    public class TokensCountTextMP : TokensCount {
    #if ODIN_INSPECTOR
        [Required]
    #endif
        [SerializeField]
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