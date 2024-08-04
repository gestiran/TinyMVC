using System.Collections;
using UnityEngine;

namespace TinyMVC.Utilities {
    [DisallowMultipleComponent]
    public sealed class SceneCoroutines : MonoBehaviour {
        private bool _isDestroyed;
        
        private void OnDestroy() => _isDestroyed = true;
        
        internal Coroutine AddCoroutine(IEnumerator enumerator) {
            if (_isDestroyed) {
                return null;
            }
            
            return StartCoroutine(enumerator);
        }
        
        internal void RemoveCoroutine(Coroutine coroutine) {
            if (_isDestroyed) {
                return;
            }
            
            StopCoroutine(coroutine);
        }
        
        internal void StopAll() {
            if (_isDestroyed) {
                return;
            }
            
            StopAllCoroutines();
        }
    }
}