using System;
using JetBrains.Annotations;

namespace TinyMVC.ReactiveFields {
    public sealed class Listener : IEquatable<Listener>, IEquatable<Action> {
        private readonly Action _action;
        internal readonly int hash;

        private Listener(Action action, int hash) {
            _action = action;
            this.hash = hash;
        }

        public static implicit operator Listener(Action action) => new Listener(action, action.GetHashCode());

        public void Invoke() => _action.Invoke();
        
        public bool Equals(Listener other) => other != null && other.hash == hash;
        
        public bool Equals(Action other) => other != null && other.GetHashCode() == hash;

        public override int GetHashCode() => hash;
    }

    public sealed class Listener<T> : IEquatable<Listener<T>>, IEquatable<Action>, IEquatable<Action<T>> {
        internal readonly int hash;
        private readonly MultipleListener<T> _action;

        private Listener(MultipleListener<T> action, int hash) {
            _action = action;
            this.hash = hash;
        }
        
        public static implicit operator Listener<T>(Action action) => new Listener<T>(_ => action(), action.GetHashCode());

        public static implicit operator Listener<T>(Action<T> action) {
            return new Listener<T>(values => {
                for (int i = 0; i < values.Length; i++) {
                    action(values[i]);   
                }
            }, action.GetHashCode());
        }

        public static implicit operator Listener<T>(MultipleListener<T> action) => new Listener<T>(action, action.GetHashCode());

        public void Invoke([NotNull] T value) => _action.Invoke(value);

        public void Invoke([NotNull] params T[] values) => _action.Invoke(values);

        public bool Equals(Listener<T> other) => other != null && other.hash == hash;
        
        public bool Equals(Action other) => other != null && other.GetHashCode() == hash;
        
        public bool Equals(Action<T> other) => other != null && other.GetHashCode() == hash;
        
        public override int GetHashCode() => hash;
    }
    
    public sealed class Listener<T1, T2> : IEquatable<Listener<T1, T2>>, IEquatable<Action>, IEquatable<Action<T1, T2>> {
        internal readonly int hash;
        private readonly Action<T1, T2> _action;

        private Listener(Action<T1, T2> action, int hash) {
            _action = action;
            this.hash = hash;
        }

        public static implicit operator Listener<T1, T2>(Action action) => new Listener<T1, T2>((_, _) => action(), action.GetHashCode());

        public static implicit operator Listener<T1, T2>(Action<T1, T2> action) => new Listener<T1, T2>(action, action.GetHashCode());

        public void Invoke([NotNull] T1 first, [NotNull] T2 second) => _action.Invoke(first, second);

        public bool Equals(Listener<T1, T2> other) => other != null && other.hash == hash;
        
        public bool Equals(Action other) => other != null && other.GetHashCode() == hash;
        
        public bool Equals(Action<T1, T2> other) => other != null && other.GetHashCode() == hash;
        
        public override int GetHashCode() => hash;
    }
    
    public sealed class Listener<T1, T2, T3> : IEquatable<Listener<T1, T2, T3>>, IEquatable<Action>, IEquatable<Action<T1, T2, T3>> {
        internal readonly int hash;
        private readonly Action<T1, T2, T3> _action;

        private Listener(Action<T1, T2, T3> action, int hash) {
            _action = action;
            this.hash = hash;
        }

        public static implicit operator Listener<T1, T2, T3>(Action action) => new Listener<T1, T2, T3>((_, _, _) => action(), action.GetHashCode());

        public static implicit operator Listener<T1, T2, T3>(Action<T1, T2, T3> action) => new Listener<T1, T2, T3>(action, action.GetHashCode());

        public void Invoke([NotNull] T1 first, [NotNull] T2 second, [NotNull] T3 third) => _action.Invoke(first, second, third);

        public bool Equals(Listener<T1, T2, T3> other) => other != null && other.hash == hash;
        
        public bool Equals(Action other) => other != null && other.GetHashCode() == hash;
        
        public bool Equals(Action<T1, T2, T3> other) => other != null && other.GetHashCode() == hash;
        
        public override int GetHashCode() => hash;
    }
}