using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyMVC.Modules.Networks.Components {
    [DisallowMultipleComponent]
    public abstract class PingText : MonoBehaviour {
        private List<int> _ping;
        private Coroutine _drawPing;
        
        private void Awake() {
            _ping = new List<int>();
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
        
        protected abstract void UpdatePing(string ping);
        
        private int GetPing() {
            int ping = 0;
            
            for (int i = 0; i < _ping.Count; i++) {
                if (_ping[i] > ping) {
                    ping = _ping[i];
                }
            }
            
            _ping.Clear();
            
            return ping;
        }
        
        private IEnumerator DrawPing() {
            while (Application.isPlaying) {
                UpdatePing($"{GetPing()} ms");
                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }
}