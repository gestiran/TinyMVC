using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace TinyMVC.Modules.ADS.Components {
    [RequireComponent(typeof(TMP_Text))]
    public sealed class TokensCountEvent : TokensCount {
        [SerializeField]
        private UnityEvent<int> _onCountChange;
        
        protected override void UpdateCount(int count) => _onCountChange.Invoke(count);
    }
}