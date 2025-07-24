// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using UnityEngine;

namespace TinyMVC.Modules.ADS {
    public sealed class ADSTokenModule : IApplicationModule {
        public event Action<int> onCountChanged;
        
        public int tokenCount { get; private set; }
        
        public ADSTokenModule() {
            tokenCount = ADSSaveUtility.LoadTokensCount(ADSParameters.LoadFromResources().initialRewardTokensCount);
        }
        
        public bool IsHaveTokens(int count = 1) => tokenCount >= count;
        
        public void AddTokens(int count) {
            tokenCount += count;
            onCountChanged?.Invoke(tokenCount);
            ADSSaveUtility.SaveTokensCount(tokenCount);
        }
        
        public void SubtractTokens(int count) {
            tokenCount = Mathf.Max(tokenCount - count, 0);
            onCountChanged?.Invoke(tokenCount);
            ADSSaveUtility.SaveTokensCount(tokenCount);
        }
        
        public void SetTokens(int count) {
            tokenCount = count;
            onCountChanged?.Invoke(tokenCount);
            ADSSaveUtility.SaveTokensCount(tokenCount);
        }
        
        public bool TrySpendTokens(int count = 1) {
            if (IsHaveTokens(count)) {
                SubtractTokens(count);
                return true;
            }
            
            return false;
        }
        
        public void TrySpendTokens(Action onSuccess, int count = 1) {
            if (IsHaveTokens(count) == false) {
                return;
            }
            
            SubtractTokens(count);
            onSuccess.Invoke();
        }
    }
}