using System;
using UnityEngine;

namespace TinyMVC.ReactiveFields {
    public sealed class LazyListener : Listener, IEquatable<LazyListener> {
        private float _lastTime;
        private readonly float _frequency;

        internal LazyListener(Action action, float frequency, int hash) : base(action, hash) => _frequency = frequency;

        internal static LazyListener New(Action action, float frequency) => new LazyListener(action, frequency, action.GetHashCode());

        internal override void Invoke() {
            float time = Time.time;

            if (time - _lastTime < _frequency) {
                return;
            }

            base.Invoke();
            _lastTime = time;
        }

        public bool Equals(LazyListener other) => other != null && other.hash == hash;
    }

    public sealed class LazyListener<T> : Listener<T>, IEquatable<LazyListener<T>> {
        private float _lastTime;
        private readonly float _frequency;

        internal LazyListener(MultipleListener<T> action, float frequency, int hash) : base(action, hash) => _frequency = frequency;

        internal static LazyListener<T> New(Action action, float frequency) => new LazyListener<T>(_ => action(), frequency, action.GetHashCode());

        internal static LazyListener<T> New(Action<T> action, float frequency) {
            return new LazyListener<T>(
                values => {
                    for (int i = 0; i < values.Length; i++) {
                        action(values[i]);
                    }
                }, frequency, action.GetHashCode()
            );
        }

        internal static LazyListener<T> New(MultipleListener<T> action, float frequency) => new LazyListener<T>(action, frequency, action.GetHashCode());

        internal override void Invoke(T value) {
            float time = Time.time;

            if (time - _lastTime < _frequency) {
                return;
            }

            base.Invoke(value);
            _lastTime = time;
        }

        internal override void Invoke(params T[] values) {
            float time = Time.time;

            if (time - _lastTime < _frequency) {
                return;
            }

            base.Invoke(values);
            _lastTime = time;
        }
        
        internal override void InvokeNull() {
            float time = Time.time;

            if (time - _lastTime < _frequency) {
                return;
            }

            base.InvokeNull();
            _lastTime = time;
        }

        public bool Equals(LazyListener<T> other) => other != null && other.hash == hash;
    }

    public sealed class LazyListener<T1, T2> : Listener<T1, T2>, IEquatable<LazyListener<T1, T2>> {
        private float _lastTime;
        private readonly float _frequency;

        internal LazyListener(Action<T1, T2> action, float frequency, int hash) : base(action, hash) => _frequency = frequency;

        internal static LazyListener<T1, T2> New(Action action, float frequency) {
            return new LazyListener<T1, T2>((_, _) => action(), frequency, action.GetHashCode());
        }

        internal static LazyListener<T1, T2> New(Action<T1, T2> action, float frequency) {
            return new LazyListener<T1, T2>(action, frequency, action.GetHashCode());
        }

        internal override void Invoke(T1 first, T2 second) {
            float time = Time.time;

            if (time - _lastTime < _frequency) {
                return;
            }

            base.Invoke(first, second);
            _lastTime = time;
        }
        
        internal override void InvokeNull(T1 first, T2 second) {
            float time = Time.time;

            if (time - _lastTime < _frequency) {
                return;
            }

            base.InvokeNull(first, second);
            _lastTime = time;
        }

        public bool Equals(LazyListener<T1, T2> other) => other != null && other.hash == hash;
    }
    
    public sealed class LazyListener<T1, T2, T3> : Listener<T1, T2, T3>, IEquatable<LazyListener<T1, T2, T3>> {
        private float _lastTime;
        private readonly float _frequency;

        internal LazyListener(Action<T1, T2, T3> action, float frequency, int hash) : base(action, hash) => _frequency = frequency;

        internal static LazyListener<T1, T2, T3> New(Action action, float frequency) {
            return new LazyListener<T1, T2, T3>((_, _, _) => action(), frequency, action.GetHashCode());
        }

        internal static LazyListener<T1, T2, T3> New(Action<T1, T2, T3> action, float frequency) {
            return new LazyListener<T1, T2, T3>(action, frequency, action.GetHashCode());
        }

        internal override void Invoke(T1 first, T2 second, T3 third) {
            float time = Time.time;

            if (time - _lastTime < _frequency) {
                return;
            }

            base.Invoke(first, second, third);
            _lastTime = time;
        }
        
        internal override void InvokeNull(T1 first, T2 second, T3 third) {
            float time = Time.time;

            if (time - _lastTime < _frequency) {
                return;
            }

            base.InvokeNull(first, second, third);
            _lastTime = time;
        }

        public bool Equals(LazyListener<T1, T2, T3> other) => other != null && other.hash == hash;
    }
}