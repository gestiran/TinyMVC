// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;
using UnityEngine.UI;

#if UNITY_PURCHASING
using System;
using UnityEngine.Purchasing;
#endif

namespace TinyMVC.Modules.IAP {
    [DisallowMultipleComponent]
    public class PriceText : MonoBehaviour {
        [SerializeField]
        private Text _price;
        
        public void Init(string productId) {
        #if UNITY_PURCHASING
            try {
                Product product = CodelessIAPStoreListener.Instance.GetProduct(productId);
                
                if (_price == null) {
                    return;
                }
                
                _price.text = product.metadata.localizedPriceString;
            } catch (Exception exception) {
                Debug.LogWarning(exception);
            }
            
        #endif
        }
    }
}