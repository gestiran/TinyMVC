using UnityEngine;
using UnityEngine.UI;

#if UNITY_PURCHASING
using System;
using UnityEngine.Purchasing;
#endif

namespace TinyMVC.Modules.IAP {
    [DisallowMultipleComponent]
    public sealed class PriceText : MonoBehaviour {
        [SerializeField]
        private Text _price;
        
        public void Init(BuyHandler handler) {
        #if UNITY_PURCHASING
            try {
                Product product = CodelessIAPStoreListener.Instance.GetProduct(handler.productId);
                
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