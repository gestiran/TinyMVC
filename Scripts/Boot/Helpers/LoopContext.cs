using System.Collections.Generic;
using TinyMVC.Loop;
using System;

namespace TinyMVC.Boot.Helpers {
    internal sealed class LoopContext {
        private List<Action> _actions;
        private List<FixedTickContext> _fixedTicks;
        private List<TickContext> _ticks;
        private List<LateTickContext> _lateTicks;
        
        private const int _CONTEXTS_CAPACITY = 16;
        private const int _TICKS_CAPACITY = 512;
        
        internal sealed class FixedTickContext : ContextLink<List<IFixedTick>> {
            public FixedTickContext(int sceneId, List<IFixedTick> context) : base(sceneId, context) { }
        }
        
        internal sealed class TickContext : ContextLink<List<ITick>> {
            public TickContext(int sceneId, List<ITick> context) : base(sceneId, context) { }
        }
        
        internal sealed class LateTickContext : ContextLink<List<ILateTick>> {
            public LateTickContext(int sceneId, List<ILateTick> context) : base(sceneId, context) { }
        }
        
        internal void Init() {
            _actions = new List<Action>(_TICKS_CAPACITY);
            _fixedTicks = new List<FixedTickContext>(_CONTEXTS_CAPACITY);
            _ticks = new List<TickContext>(_CONTEXTS_CAPACITY);
            _lateTicks = new List<LateTickContext>(_CONTEXTS_CAPACITY);
        }
        
        internal void FixedTick() {
            for (int contextId = 0; contextId < _fixedTicks.Count; contextId++) {
                List<IFixedTick> ticks = _fixedTicks[contextId].context;
                
                for (int tickId = 0; tickId < ticks.Count; tickId++) {
                    _actions.Add(ticks[tickId].FixedTick);
                }
            }
            
            Invoke(_actions);
            _actions.Clear();
        }
        
        internal void Tick() {
            for (int contextId = 0; contextId < _ticks.Count; contextId++) {
                List<ITick> ticks = _ticks[contextId].context;
                
                for (int tickId = 0; tickId < ticks.Count; tickId++) {
                    _actions.Add(ticks[tickId].Tick);
                }
            }
            
            Invoke(_actions);
            _actions.Clear();
        }
        
        internal void LateTick() {
            for (int contextId = 0; contextId < _lateTicks.Count; contextId++) {
                List<ILateTick> ticks = _lateTicks[contextId].context;
                
                for (int tickId = 0; tickId < ticks.Count; tickId++) {
                    _actions.Add(ticks[tickId].LateTick);
                }
            }
            
            Invoke(_actions);
            _actions.Clear();
        }
        
        internal void AddFixedTicks(int sceneId, List<IFixedTick> ticks) {
            if (ticks.Count <= 0) {
                return;
            }
            
            if (_fixedTicks.TryGetContext(sceneId, out List<IFixedTick> current, out int _)) {
                current.AddRange(ticks);
            } else {
                _fixedTicks.Add(new FixedTickContext(sceneId, ticks));
            }
        }
        
        internal void AddTicks(int sceneId, List<ITick> ticks) {
            if (ticks.Count <= 0) {
                return;
            }
            
            if (_ticks.TryGetContext(sceneId, out List<ITick> current, out int _)) {
                current.AddRange(ticks);
            } else {
                _ticks.Add(new TickContext(sceneId, ticks));
            }
        }
        
        internal void AddLateTicks(int sceneId, List<ILateTick> ticks) {
            if (ticks.Count <= 0) {
                return;
            }
            
            if (_lateTicks.TryGetContext(sceneId, out List<ILateTick> current, out int _)) {
                current.AddRange(ticks);
            } else {
                _lateTicks.Add(new LateTickContext(sceneId, ticks));
            }
        }
        
        internal void ConnectLoop(int sceneId, ILoop loop) {
            if (loop is IFixedTick fixedTick) {
                ConnectFixedTick(sceneId, fixedTick);
            }
            
            if (loop is ITick tick) {
                ConnectTick(sceneId, tick);
            }
            
            if (loop is ILateTick lateTick) {
                ConnectLateTick(sceneId, lateTick);
            }
        }
        
        internal void DisconnectLoop(int sceneId, ILoop loop) {
            if (loop is IFixedTick fixedTick) {
                DisconnectFixedTick(sceneId, fixedTick);
            }
            
            if (loop is ITick tick) {
                DisconnectTick(sceneId, tick);
            }
            
            if (loop is ILateTick lateTick) {
                DisconnectLateTick(sceneId, lateTick);
            }
        }
        
        internal void RemoveAllContextsWithId(int sceneId) {
            if (_fixedTicks.TryGetContext(sceneId, out List<IFixedTick> _, out int fixedTickId)) {
                _fixedTicks.RemoveAt(fixedTickId);
            }
            
            if (_ticks.TryGetContext(sceneId, out List<ITick> _, out int tickId)) {
                _ticks.RemoveAt(tickId);
            }
            
            if (_lateTicks.TryGetContext(sceneId, out List<ILateTick> _, out int lateTickId)) {
                _lateTicks.RemoveAt(lateTickId);
            }
        }
        
        private void ConnectFixedTick(int sceneId, IFixedTick tick) {
            if (_fixedTicks.TryGetContext(sceneId, out List<IFixedTick> current, out int _)) {
                current.Add(tick);
            } else {
                _fixedTicks.Add(new FixedTickContext(sceneId, new List<IFixedTick>() { tick }));
            }
        }
        
        private void ConnectTick(int sceneId, ITick tick) {
            if (_ticks.TryGetContext(sceneId, out List<ITick> current, out int _)) {
                current.Add(tick);
            } else {
                _ticks.Add(new TickContext(sceneId, new List<ITick>() { tick }));
            }
        }
        
        private void ConnectLateTick(int sceneId, ILateTick tick) {
            if (_lateTicks.TryGetContext(sceneId, out List<ILateTick> current, out int _)) {
                current.Add(tick);
            } else {
                _lateTicks.Add(new LateTickContext(sceneId, new List<ILateTick>() { tick }));
            }
        }
        
        private void DisconnectFixedTick(int sceneId, IFixedTick tick) {
            if (_fixedTicks.TryGetContext(sceneId, out List<IFixedTick> current, out int _)) {
                current.Remove(tick);
            }
        }
        
        private void DisconnectTick(int sceneId, ITick tick) {
            if (_ticks.TryGetContext(sceneId, out List<ITick> current, out int _)) {
                current.Remove(tick);
            }
        }
        
        private void DisconnectLateTick(int sceneId, ILateTick tick) {
            if (_lateTicks.TryGetContext(sceneId, out List<ILateTick> current, out int _)) {
                current.Remove(tick);
            }
        }
        
        private void Invoke(List<Action> actions) {
            foreach (Action action in actions) {
                action.Invoke();
            }
        }
    }
}