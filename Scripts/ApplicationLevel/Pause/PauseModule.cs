using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace TinyMVC.ApplicationLevel.Pause {
    public sealed class PauseModule : IApplicationModule {
        public bool isEnable { get; private set; }
        
        public event Action<bool> onApplicationPause;
        public event Action<bool> onChange;
        
        public void Initialize() {
            GameObject test = new GameObject("Pause Events");
            test.AddComponent<PauseEvents>().Init(OnApplicationPause);
            UnityObject.DontDestroyOnLoad(test);
        }
        
        public void Enable() {
            if (isEnable) {
                return;
            }
            
            Time.timeScale = 0;
            isEnable = true;
            onChange?.Invoke(isEnable);
        }
        
        public void Disable() {
            if (isEnable == false) {
                return;
            }
            
            Time.timeScale = 1;
            isEnable = false;
            onChange?.Invoke(isEnable);
        }
        
        private void OnApplicationPause(bool isPause) => onApplicationPause?.Invoke(isPause);
    }
}