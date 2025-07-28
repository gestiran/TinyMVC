// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Modules.ADS.Components {
    internal sealed class NoAdsBranchState : NoAdsBranch {
    #if ODIN_INSPECTOR
        [Required]
    #endif
        [SerializeField]
        private GameObject _noAdsActive;
        
    #if ODIN_INSPECTOR
        [Required]
    #endif
        [SerializeField]
        private GameObject _noAdsInactive;
        
    #if GOOGLE_ADS_MOBILE
        
        protected override bool IsValidState() {
            bool activeState = _noAdsActive.gameObject.activeSelf;
            bool inactiveState = _noAdsInactive.gameObject.activeSelf;
            
            if (activeState && inactiveState) {
                return false;
            }
            
            if (activeState == false && inactiveState == false) {
                return false;
            }
            
            return true;
        }
        
        protected override void ToNoAdsActive() {
            _noAdsActive.SetActive(true);
            _noAdsInactive.SetActive(false);
        }
        
        protected override void ToNoAdsInactive() {
            _noAdsActive.SetActive(false);
            _noAdsInactive.SetActive(true);
        }
        
    #endif
    }
}