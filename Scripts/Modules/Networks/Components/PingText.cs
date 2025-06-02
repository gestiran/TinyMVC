using UnityEngine;

namespace TinyMVC.Modules.Networks.Components {
    [DisallowMultipleComponent]
    public abstract class PingText : MonoBehaviour {
        private void OnEnable() => NetService.ping += UpdatePing;
        
        private void OnDisable() => NetService.ping -= UpdatePing;
        
        private void UpdatePing(int ping) => UpdatePing($"{ping} ms");
        
        protected abstract void UpdatePing(string ping);
    }
}