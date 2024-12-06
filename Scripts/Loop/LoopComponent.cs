using System;
using UnityEngine;

namespace TinyMVC.Loop {
    [DisallowMultipleComponent]
    public sealed class LoopComponent : MonoBehaviour {
        private Action _tick;
        private Action _fixedTick;
        private Action _lateTick;
        
        internal static LoopComponent Create(Action tick, Action fixedTick, Action lateTick) {
            GameObject obj = new GameObject("LoopHandler");
            DontDestroyOnLoad(obj);
            LoopComponent component = obj.AddComponent<LoopComponent>();
            component.Connect(tick, fixedTick, lateTick);
            
            return component;
        }
        
        private void Connect(Action tick, Action fixedTick, Action lateTick) {
            _tick = tick;
            _fixedTick = fixedTick;
            _lateTick = lateTick;
        }
        
        private void Update() => _tick.Invoke();
        
        private void FixedUpdate() => _fixedTick.Invoke();
        
        private void LateUpdate() => _lateTick.Invoke();
    }
}