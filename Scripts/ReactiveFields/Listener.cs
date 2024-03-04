using System;
using JetBrains.Annotations;

using NotNullSystem = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace TinyMVC.ReactiveFields {
    public class Listener : IEquatable<Listener>, IEquatable<Action> {
        private readonly Action _action;
        internal readonly int hash;

        internal Listener(Action action, int hash) {
            _action = action;
            this.hash = hash;
        }

        internal static Listener New(Action action) => new Listener(action, action.GetHashCode());

        internal virtual void Invoke() => _action.Invoke();
        
        public bool Equals(Listener other) => other != null && other.hash == hash;
        
        public bool Equals(Action other) => other != null && other.GetHashCode() == hash;

        public override int GetHashCode() => hash;
    }

    public class Listener<T> : IEquatable<Listener<T>>, IEquatable<Action>, IEquatable<Action<T>> {
        internal readonly int hash;
        private readonly MultipleListener<T> _action;

        internal Listener(MultipleListener<T> action, int hash) {
            _action = action;
            this.hash = hash;
        }

        internal static Listener<T> New(Action action) => new Listener<T>(_ => action(), action.GetHashCode());
        
        internal static Listener<T> New(Action<T> action) => new Listener<T>(values => InvokeAll(action, values), action.GetHashCode());

        private static void InvokeAll(Action<T> action, T[] values) {
            for (int i = 0; i < values.Length; i++) {
                action(values[i]);
            }
        }

        internal static Listener<T> New(MultipleListener<T> action) => new Listener<T>(action, action.GetHashCode());

        internal virtual void Invoke([NotNullSystem] T value) => _action.Invoke(value);

        internal virtual void Invoke([NotNullSystem] params T[] values) => _action.Invoke(values);

        internal virtual void InvokeNull() => _action.Invoke(new T[] { default });

        public bool Equals(Listener<T> other) => other != null && other.hash == hash;
        
        public bool Equals(Action other) => other != null && other.GetHashCode() == hash;
        
        public bool Equals(Action<T> other) => other != null && other.GetHashCode() == hash;
        
        public override int GetHashCode() => hash;
    }
    
    public class Listener<T1, T2> : IEquatable<Listener<T1, T2>>, IEquatable<Action>, IEquatable<Action<T1, T2>> {
        internal readonly int hash;
        private readonly Action<T1, T2> _action;

        internal Listener(Action<T1, T2> action, int hash) {
            _action = action;
            this.hash = hash;
        }

        internal static Listener<T1, T2> New(Action action) => new Listener<T1, T2>((_, _) => action(), action.GetHashCode());

        internal static Listener<T1, T2> New(Action<T1, T2> action) => new Listener<T1, T2>(action, action.GetHashCode());

        internal virtual void Invoke([NotNullSystem] T1 first, [NotNullSystem] T2 second) => _action.Invoke(first, second);
        
        internal virtual void InvokeNull([CanBeNull] T1 first, [CanBeNull] T2 second) => _action.Invoke(first, second);

        public bool Equals(Listener<T1, T2> other) => other != null && other.hash == hash;
        
        public bool Equals(Action other) => other != null && other.GetHashCode() == hash;
        
        public bool Equals(Action<T1, T2> other) => other != null && other.GetHashCode() == hash;
        
        public override int GetHashCode() => hash;
    }
    
    public class Listener<T1, T2, T3> : IEquatable<Listener<T1, T2, T3>>, IEquatable<Action>, IEquatable<Action<T1, T2, T3>> {
        internal readonly int hash;
        private readonly Action<T1, T2, T3> _action;

        internal Listener(Action<T1, T2, T3> action, int hash) {
            _action = action;
            this.hash = hash;
        }

        internal static Listener<T1, T2, T3> New(Action action) => new Listener<T1, T2, T3>((_, _, _) => action(), action.GetHashCode());

        internal static Listener<T1, T2, T3> New(Action<T1, T2, T3> action) => new Listener<T1, T2, T3>(action, action.GetHashCode());

        internal virtual void Invoke([NotNullSystem] T1 first, [NotNullSystem] T2 second, [NotNullSystem] T3 third) => _action.Invoke(first, second, third);
        
        internal virtual void InvokeNull([CanBeNull] T1 first, [CanBeNull] T2 second, [CanBeNull] T3 third) => _action.Invoke(first, second, third);

        public bool Equals(Listener<T1, T2, T3> other) => other != null && other.hash == hash;
        
        public bool Equals(Action other) => other != null && other.GetHashCode() == hash;
        
        public bool Equals(Action<T1, T2, T3> other) => other != null && other.GetHashCode() == hash;
        
        public override int GetHashCode() => hash;
    }
}