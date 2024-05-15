using System;
using System.Collections.Generic;

namespace TinyMVC.Entities {
    public sealed class Entity : IEquatable<Entity> {
        internal int groupId { get; }

        private readonly Dictionary<Type, IData> _components;
        private readonly int _id;
        
        private const int _CAPACITY = 8;

        internal Entity(int id, int groupId) {
            _id = id;
            this.groupId = groupId;
            _components = new Dictionary<Type, IData>(_CAPACITY);
        }

        public Entity Add<T>(T component) where T : IData {
             _components.Add(typeof(T), component);
             return this;
        }

        public Entity Add<T1, T2>(T1 component1, T2 component2) where T1 : IData where T2 : IData {
            _components.Add(typeof(T1), component1);
            _components.Add(typeof(T2), component2);
            return this;
        }

        public Entity Add<T1, T2, T3>(T1 component1, T2 component2, T3 component3) where T1 : IData where T2 : IData where T3 : IData {
            _components.Add(typeof(T1), component1);
            _components.Add(typeof(T2), component2);
            _components.Add(typeof(T3), component3);
            return this;
        }

        public Entity Remove<T>() where T : IData {
            _components.Remove(typeof(T));
            return this;
        }

        public Entity Remove<T1, T2>() where T1 : IData where T2 : IData {
            _components.Remove(typeof(T1));
            _components.Remove(typeof(T2));
            return this;
        }

        public Entity Remove<T1, T2, T3>() where T1 : IData where T2 : IData where T3 : IData {
            _components.Remove(typeof(T1));
            _components.Remove(typeof(T2));
            _components.Remove(typeof(T3));
            return this;
        }

        public Entity Clear() {
            _components.Clear();
            return this;
        }

        internal IEnumerable<T> ForEach<T>() where T : IData {
            if (_components.TryGetValue(typeof(T), out IData component)) {
                yield return (T)component;
            }
        }

        internal IEnumerable<(T1, T2)> ForEach<T1, T2>() where T1 : IData where T2 : IData {
            if (_components.TryGetValue(typeof(T1), out IData component1)) {
                if (_components.TryGetValue(typeof(T2), out IData component2)) {
                    yield return ((T1)component1, (T2)component2);
                }
            }
        }

        internal IEnumerable<(T1, T2, T3)> ForEach<T1, T2, T3>() where T1 : IData where T2 : IData where T3 : IData {
            if (_components.TryGetValue(typeof(T1), out IData component1)) {
                if (_components.TryGetValue(typeof(T2), out IData component2)) {
                    if (_components.TryGetValue(typeof(T3), out IData component3)) {
                        yield return ((T1)component1, (T2)component2, (T3)component3);
                    }
                }
            }
        }
        
        internal IEnumerable<(T1, T2, T3, T4)> ForEach<T1, T2, T3, T4>() where T1 : IData where T2 : IData where T3 : IData where T4 : IData {
            if (_components.TryGetValue(typeof(T1), out IData component1)) {
                if (_components.TryGetValue(typeof(T2), out IData component2)) {
                    if (_components.TryGetValue(typeof(T3), out IData component3)) {
                        if (_components.TryGetValue(typeof(T4), out IData component4)) {
                            yield return ((T1)component1, (T2)component2, (T3)component3, (T4)component4);
                        }
                    }
                }
            }
        }

        internal bool TryGetEntity<T>(out (Entity, T) result) where T : IData {
            if (_components.TryGetValue(typeof(T), out IData component)) {
                result = (this, (T)component);
                return true;
            }

            result = default;
            return false;
        }
        
        internal bool TryGetEntity<T1, T2>(out (Entity, T1, T2) result) where T1 : IData where T2 : IData {
            if (_components.TryGetValue(typeof(T1), out IData component1)) {
                if (_components.TryGetValue(typeof(T2), out IData component2)) {
                    result = (this, (T1)component1, (T2)component2);
                    return true;
                }
            }

            result = default;
            return false;
        }
        
        internal bool TryGetEntity<T1, T2, T3>(out (Entity, T1, T2, T3) result) where T1 : IData where T2 : IData where T3 : IData {
            if (_components.TryGetValue(typeof(T1), out IData component1)) {
                if (_components.TryGetValue(typeof(T2), out IData component2)) {
                    if (_components.TryGetValue(typeof(T3), out IData component3)) {
                        result = (this, (T1)component1, (T2)component2, (T3)component3);
                        return true;
                    }
                }
            }

            result = default;
            return false;
        }
        
        internal bool TryGetEntity<T1, T2, T3, T4>(out (Entity, T1, T2, T3, T4) result) where T1 : IData where T2 : IData where T3 : IData where T4 : IData {
            if (_components.TryGetValue(typeof(T1), out IData component1)) {
                if (_components.TryGetValue(typeof(T2), out IData component2)) {
                    if (_components.TryGetValue(typeof(T3), out IData component3)) {
                        if (_components.TryGetValue(typeof(T4), out IData component4)) {
                            result = (this, (T1)component1, (T2)component2, (T3)component3, (T4)component4);
                            return true;
                        }
                    }
                }
            }

            result = default;
            return false;
        }

        public bool Equals(Entity other) => other != null && other._id == _id;

        public override int GetHashCode() => _id;
    }
}