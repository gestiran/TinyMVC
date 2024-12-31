using System;
using UnityEngine;

namespace TinyMVC.Modules.Pause {
    [DisallowMultipleComponent]
    public sealed class PauseEvents : MonoBehaviour {
    #if UNITY_EDITOR
        [SerializeField]
        private bool _isEnablePause;
    #endif
        
        private Action<bool> _onApplicationPause;
        
        public void Init(Action<bool> onApplicationPause) {
            _onApplicationPause = onApplicationPause;
            
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.pauseStateChanged += EditorPause;
        #endif
        }
        
    #if UNITY_EDITOR
        private void EditorPause(UnityEditor.PauseState state) {
            if (_isEnablePause) {
                _onApplicationPause?.Invoke(state == UnityEditor.PauseState.Paused);
            }
        }
    #else
        private void OnApplicationPause(bool isPause) => _onApplicationPause?.Invoke(isPause);
    #endif
    }
}