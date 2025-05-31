using Sirenix.OdinInspector;

namespace TinyMVC.Modules.ADS {
    [ShowInInspector, InlineProperty, HideDuplicateReferenceBox, HideReferenceObjectPicker]
    public sealed class ADSTokensCount {
        [field: ShowInInspector, OnValueChanged("ApplyCount"), HorizontalGroup, HideLabel]
        public int count { get; private set; }
        
        public ADSTokensCount() {
            count = API<ADSTokenModule>.module.tokenCount;
            API<ADSTokenModule>.module.onCountChanged += UpdateCount;
        }
        
        ~ADSTokensCount() {
            API<ADSTokenModule>.module.onCountChanged -= UpdateCount;
        }
        
        private void UpdateCount(int value) => count = value;
        
    #if UNITY_EDITOR
        private void ApplyCount() {
            int current = API<ADSTokenModule>.module.tokenCount;
            
            if (count == current) {
                return;
            }
            
            if (count > current) {
                API<ADSTokenModule>.module.AddTokens(count - current);
            } else {
                API<ADSTokenModule>.module.SubtractTokens(current - count);
            }
        }
        
        [Button("+1"), HorizontalGroup]
        private void Add() {
            count++;
            ApplyCount();
        }
        
        [Button("-1"), HorizontalGroup]
        private void Subtract() {
            count--;
            ApplyCount();
        }
    #endif
    }
}