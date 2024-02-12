using System.Collections.Generic;
using TinyMVC.Loop;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using Unity.Profiling;
using TinyMVC.Exceptions;

using UnityObject = UnityEngine.Object;
#endif

namespace TinyMVC.Boot.Helpers {
    internal sealed class LoopContext {
        private List<FixedTickContext> _fixedTicks;
        private List<TickContext> _ticks;
        private List<LateTickContext> _lateTicks;

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        private ProfilerMarker _fixedUpdateMarker;
        private ProfilerMarker _updateMarker;
        private ProfilerMarker _lateUpdateMarker;
    #endif

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
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            _fixedUpdateMarker = new ProfilerMarker(ProfilerCategory.Scripts, "Loop.FixedUpdate");
            _updateMarker = new ProfilerMarker(ProfilerCategory.Scripts, "Loop.Update");
            _lateUpdateMarker = new ProfilerMarker(ProfilerCategory.Scripts, "Loop.LateUpdate");
        #endif

            _fixedTicks = new List<FixedTickContext>();
            _ticks = new List<TickContext>();
            _lateTicks = new List<LateTickContext>();
        }

        internal void FixedUpdate() {
            foreach (FixedTickContext context in _fixedTicks) {
                foreach (IFixedTick fixedTick in context.context) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    if (fixedTick is UnityObject unityObject) {
                        _fixedUpdateMarker.Begin(unityObject);
                    } else {
                        _fixedUpdateMarker.Begin();
                    }

                    try {
                    #endif

                        fixedTick.FixedTick();

                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    } catch (Exception exception) {
                        throw new FixedTickException(fixedTick, exception);
                    }

                    _fixedUpdateMarker.End();
                #endif
                }
            }
        }

        internal void Update() {
            foreach (TickContext context in _ticks) {
                foreach (ITick tick in context.context) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    if (tick is UnityObject unityObject) {
                        _updateMarker.Begin(unityObject);
                    } else {
                        _updateMarker.Begin();
                    }

                    try {
                    #endif

                        tick.Tick();

                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    } catch (Exception exception) {
                        throw new TickException(tick, exception);
                    }

                    _updateMarker.End();
                #endif
                }
            }
        }

        internal void LateUpdate() {
            foreach (LateTickContext context in _lateTicks) {
                foreach (ILateTick lateTick in context.context) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    if (lateTick is UnityObject unityObject) {
                        _lateUpdateMarker.Begin(unityObject);
                    } else {
                        _lateUpdateMarker.Begin();
                    }

                    try {
                    #endif

                        lateTick.LateTick();

                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    } catch (Exception exception) {
                        throw new LateTickException(lateTick, exception);
                    }

                    _lateUpdateMarker.End();
                #endif
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