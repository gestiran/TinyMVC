using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_PURCHASING
using System.Threading.Tasks;
using UnityEngine.Purchasing;
#endif

namespace TinyMVC.Modules.IAP {
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    #if UNITY_PURCHASING
    public sealed class BuyButton : CodelessIAPButton, IDisposable {
        #else
    public sealed class BuyButton : MonoBehaviour, IDisposable {
        #endif
        [SerializeField]
        private Text _price;
        
        #if UNITY_EDITOR
        [SerializeField]
        private string _productIdEditor;
        #endif
        
        [SerializeField]
        private UnityEvent _onBuySuccess;
        
        [SerializeField]
        private bool _isActive;
        
        private BuyHandler _handler;
        
        private void Awake() {
            #if UNITY_PURCHASING
            if (string.IsNullOrEmpty(productId)) {
                productId = "com.inkosgames.holein.packboosters1";
            }
            #endif
        }
        
        public void Init(BuyHandler handler) {
            _handler = handler;
            handler.onBuySuccess += _onBuySuccess.Invoke;
            
            #if UNITY_PURCHASING
            productId = handler.productId;
            
            if (TryUpdatePrice()) {
                CodelessIAPStoreListener.Instance.AddButton(this);
                _isActive = true;
            }
            
            #endif
        }
        
        public void Dispose() {
            _handler.onBuySuccess -= _onBuySuccess.Invoke;
            #if UNITY_PURCHASING
            CodelessIAPStoreListener.Instance.RemoveButton(this);
            #endif
            _isActive = false;
        }
        
        #if UNITY_PURCHASING
        protected override void OnPurchaseComplete(Product product) {
            if (!_isActive) {
                return;
            }
            
            base.OnPurchaseComplete(product);
            SendToHandler();
        }
        
        protected override Button GetPurchaseButton() {
            if (button == null) {
                button = GetComponent<Button>();
            }
            
            return button;
        }
        
        protected override void AddButtonToCodelessListener() {
            // Do nothing
        }
        
        protected override void RemoveButtonToCodelessListener() {
            // Do nothing
        } 
        
        private async void SendToHandler() {
            await Task.Yield();
            _handler.Purchase();
        }
        
        private bool TryUpdatePrice() {
            try {
                Product product = CodelessIAPStoreListener.Instance.GetProduct(productId);
                
                if (_price == null) {
                    return false;
                }
                
                _price.text = product.metadata.localizedPriceString;
                
                return true;
            } catch (Exception exception) {
                Debug.LogWarning(exception);
                
                return false;
            }
        }
        
        #if UNITY_EDITOR
        
        private void Update() => _productIdEditor = productId;
        
        #endif
        #endif
    }
}