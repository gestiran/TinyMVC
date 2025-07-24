// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using UnityEngine;

namespace TinyMVC.Modules.ADS.Components {
    [DisallowMultipleComponent]
    public sealed class BannerOverride : MonoBehaviour {
        [field: SerializeField]
        public bool visibleWithBanner;
        
    #if GOOGLE_ADS_MOBILE
        private void Awake() => UpdateRequest(false);
        
        private void OnEnable() {
            UpdateRequest(API<ADSModule>.module.isVisibleBanner);
            
            API<ADSModule>.module.onBannerShow += OnBannerShow;
            API<ADSModule>.module.onBannerHide += OnBannerHide;
        }
        
        private void OnDisable() {
            StopAllCoroutines();
            API<ADSModule>.module.onBannerShow -= OnBannerShow;
            API<ADSModule>.module.onBannerHide -= OnBannerHide;
        }
        
        private void OnBannerShow() => UpdateRequest(true);
        
        private void OnBannerHide() => UpdateRequest(false);
        
        private void UpdateRequest(bool isVisibleBanner) {
            try {
                if (gameObject.activeInHierarchy) {
                    StopAllCoroutines();
                    StartCoroutine(UpdateProcess(isVisibleBanner));
                } else {
                    UpdateState(isVisibleBanner);
                }
            } catch (Exception exception) {
                Debug.LogWarning(exception);
            }
        }
        
        private void UpdateState(bool isVisibleBanner) => gameObject.SetActive(isVisibleBanner == visibleWithBanner);
        
        private bool IsValidState() {
            if (API<ADSModule>.module.isVisibleBanner && gameObject.activeSelf != visibleWithBanner) {
                return false;
            }
            
            if (API<ADSModule>.module.isVisibleBanner == false && gameObject.activeSelf == visibleWithBanner) {
                return false;
            }
            
            return true;
        }
        
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