// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

namespace TinyMVC.Modules.ADS.Components {
    public sealed class TokensCountTextMPPrefix : TokensCountTextMP {
        [SerializeField]
        private string _prefix;
        
        protected override string ConvertToText(int count) => $"{_prefix}{base.ConvertToText(count)}";
    }
}