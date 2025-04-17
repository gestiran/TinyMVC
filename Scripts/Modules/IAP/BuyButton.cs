using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace TinyMVC.Modules.IAP {
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
#if UNITY_PURCHASING
    public sealed class BuyButton : CodelessIAPButton, IDisposable, ISelfValidator {
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
        
    #if UNITY_PURCHASING_FAKE && !UNITY_PURCHASING
        private Button button;
    #endif
        
        private void Awake() {
        #if UNITY_PURCHASING_FAKE
            button = GetComponent<Button>();
            
            if (_price != null) {
                _price.text = "Fake";
            }
        #elif UNITY_PURCHASING
            if (string.IsNullOrEmpty(productId)) {
                productId = "com.inkosgames.holein.packboosters1";
            }
        #endif
        }
        
        public void Init(BuyHandler handler) {
            _handler = handler;
            
            handler.onBuySuccess += _onBuySuccess.Invoke;
            handler.onRestoreSuccess += _onBuySuccess.Invoke;
            
        #if UNITY_PURCHASING_FAKE
            if (button != null) {
                button.onClick.AddListener(SendToHandler);
            }
        #elif UNITY_PURCHASING
            productId = handler.productId;
            
            if (TryUpdatePrice()) {
                CodelessIAPStoreListener.Instance.AddButton(this);
                _isActive = true;
            }
        #endif
        }
        
        public void Dispose() {
            _handler.onBuySuccess -= _onBuySuccess.Invoke;
            _handler.onRestoreSuccess -= _onBuySuccess.Invoke;
            
        #if UNITY_PURCHASING_FAKE
            if (button != null && _handler != null) {
                button.onClick.RemoveListener(SendToHandler);
            }
        #elif UNITY_PURCHASING
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
        
        private async void SendToHandler() {
            await Task.Yield();
            _handler.Purchase();
        }
        
        public void Validate(SelfValidationResult result) {
        #if UNITY_EDITOR
            
            if (_price == null) {
                result.AddWarning("Can't find price text!");
            } else if (_price.resizeTextForBestFit == false) {
                result.AddError("Price text isn't adaptive!").WithFix("Apply adaptive.", FixAdaptiveText);
            }
            
        #endif
        }
    #if UNITY_EDITOR
        private void FixAdaptiveText() {
            _price.resizeTextForBestFit = true;
            
            _price.resizeTextMaxSize = _price.fontSize;
            _price.resizeTextMinSize = 5;
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
    #endif
    }
}