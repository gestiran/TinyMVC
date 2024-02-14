using System.Collections.Generic;

namespace TinyMVC.ReactiveFields {
    public interface IObservedList<T> : IObservedList {
        internal List<Listener<T>> onAdd { get; }

        internal List<Listener<T>> onRemove { get; }
    }

    public interface IObservedList {
        internal List<Listener> onClear { get; }
    }
}