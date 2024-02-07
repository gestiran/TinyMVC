using System.Collections.Generic;
using TinyMVC.Loop;

namespace TinyMVC.Boot.Helpers {
    internal sealed class LoopContext {
        private List<FixedTickContext> _fixedTicks;
        private List<TickContext> _ticks;
        private List<LateTickContext> _lateTicks;

        private sealed class FixedTickContext : ContextLink<List<IFixedTick>> {
            public FixedTickContext(int sceneId, List<IFixedTick> context) : base(sceneId, context) { }
        }

        private sealed class TickContext : ContextLink<List<ITick>> {
            public TickContext(int sceneId, List<ITick> context) : base(sceneId, context) { }
        }

        private sealed class LateTickContext : ContextLink<List<ILateTick>> {
            public LateTickContext(int sceneId, List<ILateTick> context) : base(sceneId, context) { }
        }

        internal void Init() {
            _fixedTicks = new List<FixedTickContext>();
            _ticks = new List<TickContext>();
            _lateTicks = new List<LateTickContext>();
        }

        internal void FixedUpdate() {
            foreach (FixedTickContext context in _fixedTicks) {
                foreach (IFixedTick fixedTick in context.context) {
                    fixedTick.FixedTick();
                }
            }
        }

        internal void Update() {
            for (int contextId = 0; contextId < _ticks.Count; contextId++) {
                for (int tickId = 0; tickId < _ticks[contextId].context.Count; tickId++) {
                    _ticks[contextId].context[tickId].Tick();
                }
            }
        }

        internal void LateUpdate() {
            for (int contextId = 0; contextId < _lateTicks.Count; contextId++) {
                for (int tickId = 0; tickId < _lateTicks[contextId].context.Count; tickId++) {
                    _lateTicks[contextId].context[tickId].LateTick();
                }
            }
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
    }
}