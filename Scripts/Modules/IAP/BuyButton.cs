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
            
            UpdatePrice();
            CodelessIAPStoreListener.Instance.AddButton(this);
            #endif
            Active();
        }
        
        public void Dispose() {
            _handler.onBuySuccess -= _onBuySuccess.Invoke;
            #if UNITY_PURCHASING
            CodelessIAPStoreListener.Instance.RemoveButton(this);
            #endif
            Inactive();
        }
        
        public void Active() => _isActive = true;
        
        public void Inactive() => _isActive = false;
        
        #if UNITY_PURCHASING
        protected override void OnPurchaseComplete(Product product) {
            base.OnPurchaseComplete(product);
            
            if (!_isActive) {
                return;
            }
            
            SendToHandler();
        }
        
        protected override Button GetPurchaseButton() {
            if (button == null) {
                button = GetComponent<Button>();
            }
            
            return button;
        }
        
        protected override void AddButtonToCodelessListener() { } // Do nothing
        
        protected override void RemoveButtonToCodelessListener() { } // Do nothing
        
        private async void SendToHandler() {
            await Task.Yield();
            _handler.Purchase();
        }
        
        private void ChangeLayerFromAllChildren(GameObject target, int layer) {
            Transform targetTransform = target.transform;
            int children = targetTransform.childCount;
            
            for (int childId = 0; childId < children; childId++) {
                ChangeLayerFromAllChildrenNonRecursive(targetTransform.GetChild(childId).gameObject, layer);
            }
            
            target.layer = layer;
        }
        
        private void ChangeLayerFromAllChildrenNonRecursive(GameObject target, int layer) => ChangeLayerFromAllChildren(target, layer);
        
        private void UpdatePrice() {
            Product product = CodelessIAPStoreListener.Instance.GetProduct(productId);
            
            if (_price == null) {
                return;
            }
            
            _price.text = product.metadata.localizedPriceString;
        }
        
        #if UNITY_EDITOR
        
        private void Update() => _productIdEditor = productId;
        
        #endif
        #endif
    }
}