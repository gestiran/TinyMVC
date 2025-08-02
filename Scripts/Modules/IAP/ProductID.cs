// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using UnityEngine;

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Modules.IAP {
#if UNITY_EDITOR && ODIN_INSPECTOR
    [InlineProperty, HideDuplicateReferenceBox, HideReferenceObjectPicker]
#endif
    [Serializable]
    public sealed class ProductID {
    #if UNITY_EDITOR && ODIN_INSPECTOR
        [field: HideLabel, ValueDropdown("GetAllProducts")]
    #endif
        [field: SerializeField]
        public string value { get; private set; }
        
        public static implicit operator string(ProductID other) => other.value;
        
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