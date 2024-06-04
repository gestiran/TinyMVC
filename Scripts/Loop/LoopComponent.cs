using System;
using TinyMVC.Boot;
using UnityEngine;

namespace TinyMVC.Loop {
    [DisallowMultipleComponent]
    public sealed class LoopComponent : MonoBehaviour {
        private Action _tick;
        private Action _fixedTick;
        private Action _lateTick;
        private DateTime _lastFrameTime;
        private float _lastDelta;
        
        private const float _MIN_DELTA = 0.1f;
        private const float _LAST_DELTA_IMPACT = 0.5f;
        
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
            _lastFrameTime = DateTime.Now;
            _lastDelta = Time.unscaledDeltaTime;
            ProjectContext.deltaTime = _lastDelta * Time.timeScale;
            ProjectContext.unscaledDeltaTime = _lastDelta;
        }
        
        private void Update() {
            float delta = CalculateDelta();
            ProjectContext.deltaTime = delta * Time.timeScale;
            ProjectContext.unscaledDeltaTime = delta;
            _tick.Invoke();
        }
        
        private void FixedUpdate() => _fixedTick.Invoke();
        
        private void LateUpdate() => _lateTick.Invoke();
        
        private float CalculateDelta() {
            DateTime now = DateTime.Now;
            float delta = (float)(now.Subtract(_lastFrameTime).TotalMilliseconds * 0.001);
            _lastFrameTime = now;
            delta = Mathf.Lerp(delta, _lastDelta, _LAST_DELTA_IMPACT);
            
            if (delta > _MIN_DELTA) {
                delta = Time.deltaTime;
            }
            
            _lastDelta = delta;
            
            return _lastDelta;
        }
    }
}