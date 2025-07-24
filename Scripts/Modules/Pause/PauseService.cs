// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace TinyMVC.Modules.Pause {
    public static class PauseService {
        public static bool isEnable { get; private set; }
        
        public static event Action<bool> onApplicationPause;
        public static event Action<bool> onChange;
        
        public static void Initialize() {
            GameObject test = new GameObject("Pause Events");
            test.AddComponent<PauseEvents>().Init(OnApplicationPause);
            UnityObject.DontDestroyOnLoad(test);
        }
        
        public static void Enable(float timeScale = 0) {
            if (isEnable) {
                return;
            }
            
            Time.timeScale = timeScale;
            isEnable = true;
            onChange?.Invoke(isEnable);
        }
        
        public static void Disable(float timeScale = 1) {
            if (isEnable == false) {
                return;
            }
            
            Time.timeScale = timeScale;
            isEnable = false;
            onChange?.Invoke(isEnable);
        }
        
        private static void OnApplicationPause(bool isPause) => onApplicationPause?.Invoke(isPause);
    }
}