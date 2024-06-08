using System;
using UnityEngine;

namespace TinyMVC.ApplicationLevel.Pause {
    [DisallowMultipleComponent]
    public sealed class PauseEvents : MonoBehaviour {
        private Action<bool> _onApplicationPause;
        
        public void Init(Action<bool> onApplicationPause) => _onApplicationPause = onApplicationPause;
        
        #if !UNITY_EDITOR
        private void OnApplicationPause(bool isPause) => _onApplicationPause?.Invoke(isPause);
        #endif
    }
}