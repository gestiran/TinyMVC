// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Modules.IAP {
    [DisallowMultipleComponent]
    public sealed class PriceTextAuto : PriceText {
    #if ODIN_INSPECTOR
        [ValueDropdown("GetAllProducts")]
    #endif
        [SerializeField]
        private string _productId;
        
        private void Start() => Init(_productId);
        
    #if UNITY_EDITOR && ODIN_INSPECTOR
        
        private ValueDropdownList<string> GetAllProducts() {
            string[] products = BuyHandler.LoadPurchasesValues();
            
            ValueDropdownList<string> values = new ValueDropdownList<string>();
            
            for (int i = 0; i < products.Length; i++) {
                values.Add(products[i], products[i]);
            }
            
            return values;
        }
        
    #endif
    }
}