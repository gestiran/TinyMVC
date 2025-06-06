using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyMVC.Modules.Networks.Components {
    [DisallowMultipleComponent]
    public abstract class PingText : MonoBehaviour {
        private List<int> _ping;
        private Coroutine _drawPing;
        
        private void Awake() {
            _ping = new List<int>(256);
        }
        
        private void OnEnable() {
            NetService.ping += UpdatePing;
            
            _drawPing = StartCoroutine(DrawPing());
        }
        
        private void OnDisable() {
            NetService.ping -= UpdatePing;
            
            if (_drawPing != null) {
                StopCoroutine(_drawPing);
            }
        }
        
        private void UpdatePing(int ping) => _ping.Add(ping);
        
        protected abstract void UpdateText(string ping);
        
        private int GetPing() {
            if (_ping.Count == 0) {
                return 0;
            }
            
            int ping = 0;
            
            foreach (int value in _ping) {
                ping += value;
            }
            
            ping /= _ping.Count;
            return ping;
        }
        
        private IEnumerator DrawPing() {
            while (Application.isPlaying) {
                int ping = GetPing();
                
                if (ping > 0) {
                    UpdateText($"Ping: {ping} ms");
                } else {
                    UpdateText("Disconnected");
                }
                
                _ping.Clear();
                
                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }
}