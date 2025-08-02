// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

namespace TinyMVC.Modules.IAP {
    [DisallowMultipleComponent]
    public sealed class PriceTextAuto : PriceText {
        [SerializeField]
        private ProductID _productId;
        
        private void Start() => Init(_productId);
    }
}