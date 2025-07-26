// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

namespace TinyMVC.Modules.ADS.Components {
    [DisallowMultipleComponent]
    public abstract class TokensCount : MonoBehaviour {
        private void OnEnable() {
            UpdateCount(API<ADSTokenModule>.module.tokenCount);
            
            API<ADSTokenModule>.module.onCountChanged += UpdateCount;
        }
        
        private void OnDisable() {
            API<ADSTokenModule>.module.onCountChanged -= UpdateCount;
        }
        
        internal abstract void UpdateCount(int count);
    }
}