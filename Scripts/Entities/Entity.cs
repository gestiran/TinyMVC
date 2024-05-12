using System;
using System.Collections.Generic;

namespace TinyMVC.Entities {
    public abstract class Entity {
        private readonly Dictionary<Type, IComponentData> _components;

        private const int _CAPACITY = 8;

        internal Entity() => _components = new Dictionary<Type, IComponentData>(_CAPACITY);

        public Entity Add<T>(T component) where T : IComponentData {
             _components.Add(typeof(T), component);
             return this;
        }

        public Entity Add<T1, T2>(T1 component1, T2 component2) where T1 : IComponentData where T2 : IComponentData {
            _components.Add(typeof(T1), component1);
            _components.Add(typeof(T2), component2);
            return this;
        }

        public Entity Add<T1, T2, T3>(T1 component1, T2 component2, T3 component3) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData {
            _components.Add(typeof(T1), component1);
            _components.Add(typeof(T2), component2);
            _components.Add(typeof(T3), component3);
            return this;
        }

        public Entity Remove<T>() where T : IComponentData {
            _components.Remove(typeof(T));
            return this;
        }

        public Entity Remove<T1, T2>() where T1 : IComponentData where T2 : IComponentData {
            _components.Remove(typeof(T1));
            _components.Remove(typeof(T2));
            return this;
        }

        public Entity Remove<T1, T2, T3>() where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData {
            _components.Remove(typeof(T1));
            _components.Remove(typeof(T2));
            _components.Remove(typeof(T3));
            return this;
        }

        public Entity Clear() {
            _components.Clear();
            return this;
        }

        internal IEnumerable<T> ForEach<T>() where T : IComponentData {
            if (_components.TryGetValue(typeof(T), out IComponentData component)) {
                yield return (T)component;
            }
        }

        internal IEnumerable<(T1, T2)> ForEach<T1, T2>() where T1 : IComponentData where T2 : IComponentData {
            if (_components.TryGetValue(typeof(T1), out IComponentData component1)) {
                if (_components.TryGetValue(typeof(T2), out IComponentData component2)) {
                    yield return ((T1)component1, (T2)component2);
                }
            }
        }

        internal IEnumerable<(T1, T2, T3)> ForEach<T1, T2, T3>() where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData {
            if (_components.TryGetValue(typeof(T1), out IComponentData component1)) {
                if (_components.TryGetValue(typeof(T2), out IComponentData component2)) {
                    if (_components.TryGetValue(typeof(T3), out IComponentData component3)) {
                        yield return ((T1)component1, (T2)component2, (T3)component3);
                    }
                }
            }
        }
        
        internal IEnumerable<(T1, T2, T3, T4)> ForEach<T1, T2, T3, T4>() where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData {
            if (_components.TryGetValue(typeof(T1), out IComponentData component1)) {
                if (_components.TryGetValue(typeof(T2), out IComponentData component2)) {
                    if (_components.TryGetValue(typeof(T3), out IComponentData component3)) {
                        if (_components.TryGetValue(typeof(T4), out IComponentData component4)) {
                            yield return ((T1)component1, (T2)component2, (T3)component3, (T4)component4);
                        }
                    }
                }
            }
        }
    }
}