// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace TinyMVC.Modules.IAP {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
#if UNITY_PURCHASING && !UNITY_PURCHASING_FAKE
    public sealed class BuyButton : CodelessIAPButton, IDisposable
#if ODIN_INSPECTOR && ODIN_VALIDATOR
                                  , ISelfValidator
#endif
    {
    #else
    public sealed class BuyButton : MonoBehaviour, IDisposable
#if ODIN_INSPECTOR && ODIN_VALIDATOR
                                  , ISelfValidator 
#endif
    {
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
        private CancellationTokenSource _cancellation;
        
    #if UNITY_PURCHASING_FAKE
        [Serializable] public class OnProductFetchedEvent : UnityEvent<Product> { }
        [Serializable] public class OnPurchaseCompletedEvent : UnityEvent<Product> { }
        [Serializable] public class OnPurchaseFailedEvent : UnityEvent<Product, UnityEngine.Purchasing.Extension.PurchaseFailureDescription> { }
        [Serializable] public class OnTransactionsRestoredEvent : UnityEvent<bool, string> { }
        [HideInInspector, HideInEditorMode, ReadOnly] public string productId;
        public CodelessButtonType buttonType = CodelessButtonType.Purchase;
        public bool consumePurchase = true;
        public OnTransactionsRestoredEvent onTransactionsRestored;
        public OnPurchaseCompletedEvent onPurchaseComplete;
        public OnPurchaseFailedEvent onPurchaseFailed;
        public OnProductFetchedEvent onProductFetched;
        public Button button;
    #endif
        
        private void Awake() {
        #if UNITY_PURCHASING_FAKE
            button = GetComponent<Button>();
            
            if (_price != null) {
                _price.text = "Fake";
            }
        #endif
            
        #if UNITY_PURCHASING && !UNITY_PURCHASING_FAKE
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
            productId = handler.productId;

            if (button == null) {
                button = GetComponent<Button>();
            }
            
            if (button != null) {
                button.onClick.AddListener(SendToHandler);
            }
        #elif UNITY_PURCHASING
            productId = handler.productId;
            
            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
            }
            
            _cancellation = new CancellationTokenSource();
            UpdatePriceProcess(Activate, _cancellation.Token).Forget();
            
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
            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
                _cancellation = null;
            }
            
            if (_isActive) {
                CodelessIAPStoreListener.Instance.RemoveButton(this);
            }
        #endif
            
            _isActive = false;
        }
        
    #if UNITY_PURCHASING && !UNITY_PURCHASING_FAKE
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
        
        private async UniTask UpdatePriceProcess(Action onSuccess, CancellationToken cancellation) {
            if (_price == null) {
                return;
            }
            
            try {
                while (CodelessIAPStoreListener.initializationComplete == false) {
                    await UniTask.Delay(1000, DelayType.Realtime, PlayerLoopTiming.Update, cancellation);
                }
                
                Product product = CodelessIAPStoreListener.Instance.GetProduct(productId);
                _price.text = product.metadata.localizedPriceString;
                onSuccess.Invoke();
            } catch (Exception exception) {
                Debug.LogWarning(exception);
            }
        }
        
        private void Activate() {
            CodelessIAPStoreListener.Instance.AddButton(this);
            _isActive = true;
            
            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
                _cancellation = null;
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
        
    #if ODIN_INSPECTOR && ODIN_VALIDATOR
        public void Validate(SelfValidationResult result) {
        #if UNITY_EDITOR
            
            if (_price == null) {
                result.AddWarning("Can't find price text!");
            } else if (_price.resizeTextForBestFit == false) {
                result.AddError("Price text isn't adaptive!").WithFix("Apply adaptive.", FixAdaptiveText);
            }
            
        #endif
        }
    #endif
        
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