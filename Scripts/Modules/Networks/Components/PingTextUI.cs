// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;
using UnityEngine.UI;

namespace TinyMVC.Modules.Networks.Components {
    [RequireComponent(typeof(Text))]
    public sealed class PingTextUI : PingText {
        [field: SerializeField]
        private Text _thisText;
        
        protected override void UpdateText(string ping) => _thisText.text = ping;
        
    #if UNITY_EDITOR
        
        [ContextMenu("Soft Reset")]
        private void Reset() {
            _thisText = GetComponent<Text>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
    #endif
    }
}