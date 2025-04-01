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
            public FixedTickContext(string contextKey, List<IFixedTick> context) : base(contextKey, context) { }
        }
        
        internal sealed class TickContext : ContextLink<List<ITick>> {
            public TickContext(string contextKey, List<ITick> context) : base(contextKey, context) { }
        }
        
        internal sealed class LateTickContext : ContextLink<List<ILateTick>> {
            public LateTickContext(string contextKey, List<ILateTick> context) : base(contextKey, context) { }
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
        
        internal void AddFixedTicks(string contextKey, List<IFixedTick> ticks) {
            if (ticks.Count <= 0) {
                return;
            }
            
            if (_fixedTicks.TryGetContext(contextKey, out List<IFixedTick> current, out int _)) {
                current.AddRange(ticks);
            } else {
                _fixedTicks.Add(new FixedTickContext(contextKey, ticks));
            }
        }
        
        internal void AddTicks(string contextKey, List<ITick> ticks) {
            if (ticks.Count <= 0) {
                return;
            }
            
            if (_ticks.TryGetContext(contextKey, out List<ITick> current, out int _)) {
                current.AddRange(ticks);
            } else {
                _ticks.Add(new TickContext(contextKey, ticks));
            }
        }
        
        internal void AddLateTicks(string contextKey, List<ILateTick> ticks) {
            if (ticks.Count <= 0) {
                return;
            }
            
            if (_lateTicks.TryGetContext(contextKey, out List<ILateTick> current, out int _)) {
                current.AddRange(ticks);
            } else {
                _lateTicks.Add(new LateTickContext(contextKey, ticks));
            }
        }
        
        internal void ConnectLoop(string contextKey, ILoop loop) {
            if (loop is IFixedTick fixedTick) {
                ConnectFixedTick(contextKey, fixedTick);
            }
            
            if (loop is ITick tick) {
                ConnectTick(contextKey, tick);
            }
            
            if (loop is ILateTick lateTick) {
                ConnectLateTick(contextKey, lateTick);
            }
        }
        
        internal void DisconnectLoop(string contextKey, ILoop loop) {
            if (loop is IFixedTick fixedTick) {
                DisconnectFixedTick(contextKey, fixedTick);
            }
            
            if (loop is ITick tick) {
                DisconnectTick(contextKey, tick);
            }
            
            if (loop is ILateTick lateTick) {
                DisconnectLateTick(contextKey, lateTick);
            }
        }
        
        internal void RemoveAllContextsWithId(string contextKey) {
            if (_fixedTicks.TryGetContext(contextKey, out List<IFixedTick> _, out int fixedTickId)) {
                _fixedTicks.RemoveAt(fixedTickId);
            }
            
            if (_ticks.TryGetContext(contextKey, out List<ITick> _, out int tickId)) {
                _ticks.RemoveAt(tickId);
            }
            
            if (_lateTicks.TryGetContext(contextKey, out List<ILateTick> _, out int lateTickId)) {
                _lateTicks.RemoveAt(lateTickId);
            }
        }
        
        private void ConnectFixedTick(string contextKey, IFixedTick tick) {
            if (_fixedTicks.TryGetContext(contextKey, out List<IFixedTick> current, out int _)) {
                current.Add(tick);
            } else {
                _fixedTicks.Add(new FixedTickContext(contextKey, new List<IFixedTick>() { tick }));
            }
        }
        
        private void ConnectTick(string contextKey, ITick tick) {
            if (_ticks.TryGetContext(contextKey, out List<ITick> current, out int _)) {
                current.Add(tick);
            } else {
                _ticks.Add(new TickContext(contextKey, new List<ITick>() { tick }));
            }
        }
        
        private void ConnectLateTick(string contextKey, ILateTick tick) {
            if (_lateTicks.TryGetContext(contextKey, out List<ILateTick> current, out int _)) {
                current.Add(tick);
            } else {
                _lateTicks.Add(new LateTickContext(contextKey, new List<ILateTick>() { tick }));
            }
        }
        
        private void DisconnectFixedTick(string contextKey, IFixedTick tick) {
            if (_fixedTicks.TryGetContext(contextKey, out List<IFixedTick> current, out int _)) {
                current.Remove(tick);
            }
        }
        
        private void DisconnectTick(string contextKey, ITick tick) {
            if (_ticks.TryGetContext(contextKey, out List<ITick> current, out int _)) {
                current.Remove(tick);
            }
        }
        
        private void DisconnectLateTick(string contextKey, ILateTick tick) {
            if (_lateTicks.TryGetContext(contextKey, out List<ILateTick> current, out int _)) {
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