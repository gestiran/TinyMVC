// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

namespace TinyMVC.Modules.ADS.Components {
    internal sealed class NoAdsState : NoAdsBranch {
        [SerializeField]
        private bool _isActive;
        
    #if GOOGLE_ADS_MOBILE
        
        protected override bool IsValidState() => true;
        
        protected override void ToNoAdsActive() => gameObject.SetActive(_isActive);
        
        protected override void ToNoAdsInactive() => gameObject.SetActive(_isActive == false);
        
    #endif
    }
}