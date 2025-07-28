// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;
using UnityEngine.Events;

namespace TinyMVC.Modules.ADS.Components {
    internal sealed class NoAdsBranchEvent : NoAdsBranch {
        [SerializeField]
        private UnityEvent _onNoAdsActive;
        
        [SerializeField]
        private UnityEvent _onNoAdsInactive;
        
    #if GOOGLE_ADS_MOBILE
        
        protected override bool IsValidState() => true;
        
        protected override void ToNoAdsActive() => _onNoAdsActive.Invoke();
        
        protected override void ToNoAdsInactive() => _onNoAdsInactive.Invoke();
        
    #endif
    }
}