// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace TinyMVC.Modules.IAP {
    public abstract class BuyHandler {
        public string productId => _productId;
        
        public event Action onBuySuccess;
        public event Action onRestoreSuccess;
        
        protected readonly string _productId;
        
        protected BuyHandler(string productId) {
        #if UNITY_EDITOR
            ValidateProductID(productId);
        #endif
            
            _productId = productId;
        }
        
        protected void OnBuySuccess() {
            onBuySuccess?.Invoke();
            CodelessIAPModule.OnBuySuccess(_productId);
        }
        
        protected void OnRestoreSuccess() => onRestoreSuccess?.Invoke();
        
        public abstract void Purchase(bool markAsPurchased = true);
        
        public abstract void Restore(bool markAsPurchased = true);
        
        public abstract void Confiscate();
        
        public abstract bool IsPurchased();
        
    #if UNITY_PURCHASING
        public virtual void CheckAndRestore(CodelessIAPStoreListener store) {
            if (store.HasProductInCatalog(_productId)) {
                Product product = store.GetProduct(_productId);
                
                if (product != null) {
                    if (product.hasReceipt) {
                        if (!IsPurchased()) {
                            Restore();
                        }
                    } else if (IsPurchased()) {
                        Confiscate();
                    }
                }
            } else if (IsPurchased()) {
                Confiscate();
            }
        }
    #endif
        
    #if UNITY_EDITOR
        public virtual void CheckAndRestoreDebug(string[] store) {
            if (ContainsKey(store, _productId)) {
                if (!IsPurchased()) {
                    Restore();
                }
            } else if (IsPurchased()) {
                Confiscate();
            }
        }
        
        private bool ContainsKey(string[] values, string value) {
            for (int i = 0; i < values.Length; i++) {
                if (values[i].Equals(value)) {
                    return true;
                }
            }
            
            return false;
        }
        
        private void ValidateProductID(string id) {
            if (string.IsNullOrEmpty(id)) {
                return;
            }
            
            string[] products = LoadPurchasesValues();
            bool isFind = false;
            
            for (int subProductId = 0; subProductId < products.Length; subProductId++) {
                if (products[subProductId].Equals(id)) {
                    isFind = true;
                    
                    break;
                }
            }
            
            if (!isFind) {
                Debug.LogError($"BuyHandler.ValidateProductID - Product with ID {id} not found!");
            }
        }
        
        public static string[] LoadPurchasesValues() {
            IAPParameters parameters = IAPParameters.LoadFromResources();
            TextAsset iapProducts = Resources.Load<TextAsset>(parameters.pathToProductCatalog);
            
            if (iapProducts == null) {
                return Array.Empty<string>();
            }
            
            string[] data = iapProducts.text.Split(parameters.productSeparatorId);
            
            for (int dataId = 1; dataId < data.Length; dataId++) {
                data[dataId] = $"{parameters.productSeparatorId}{data[dataId].Substring(0, data[dataId].IndexOf('"'))}";
            }
            
            List<string> list = data.ToList();
            list.RemoveAt(0);
            
            return list.ToArray();
        }
        
    #endif
    }
}