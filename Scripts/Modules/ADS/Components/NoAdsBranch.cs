// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using UnityEngine;

namespace TinyMVC.Modules.ADS.Components {
    [DisallowMultipleComponent]
    internal abstract class NoAdsBranch : MonoBehaviour {
        private Coroutine _updateProcess;
        
    #if GOOGLE_ADS_MOBILE
        private void Awake() => UpdateRequest(API<ADSModule>.module.isNoADS);
        
        private void OnEnable() {
            UpdateRequest(API<ADSModule>.module.isNoADS);
            
            API<ADSModule>.module.noAdsStateChange += UpdateRequest;
        }
        
        private void OnDisable() {
            StopAllCoroutines();
            API<ADSModule>.module.noAdsStateChange -= UpdateRequest;
        }
        
        private void UpdateRequest(bool isNoAds) {
            try {
                if (gameObject.activeInHierarchy == false) {
                    return;
                }
                
                StopUpdateProcess();
                _updateProcess = StartCoroutine(UpdateProcess(isNoAds));
            } catch (Exception exception) {
                Debug.LogWarning(exception);
            }
        }
        
        private void StopUpdateProcess() {
            if (_updateProcess != null) {
                StopCoroutine(_updateProcess);
            }
            
            _updateProcess = null;
        }
        
        private void UpdateState(bool isNoAds) {
            if (isNoAds) {
                ToNoAdsActive();
            } else {
                ToNoAdsInactive();
            }
        }
        
        protected abstract bool IsValidState();
        
        protected abstract void ToNoAdsActive();
        
        protected abstract void ToNoAdsInactive();
        
        private IEnumerator UpdateProcess(bool isLoaded) {
            do {
                try {
                    UpdateState(isLoaded);
                } catch (Exception exception) {
                    Debug.LogWarning(exception);
                }
                
                yield return new WaitForEndOfFrame();
            } while (IsValidState() == false);
        }
        
    #endif
    }
}