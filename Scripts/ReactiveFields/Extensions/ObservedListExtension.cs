using System;
using TinyMVC.Loop;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ObservedListExtension {
    #region Add

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action listener, float frequency) {
            list.onAdd.Add(LazyListener<T>.New(listener, frequency));

            return list;
        }

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action listener, float frequency, UnloadPool pool) {
            list.onAdd.Add(LazyListener<T>.New(listener, frequency));
            pool.Add(new UnloadAction(() => list.RemoveOnAddListener(listener)));

            return list;
        }

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action<T> listener, float frequency) {
            list.onAdd.Add(LazyListener<T>.New(listener, frequency));

            return list;
        }

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action<T> listener, float frequency, UnloadPool pool) {
            list.onAdd.Add(LazyListener<T>.New(listener, frequency));
            pool.Add(new UnloadAction(() => list.RemoveOnAddListener(listener)));

            return list;
        }

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, MultipleListener<T> listener, float frequency) {
            list.onAdd.Add(LazyListener<T>.New(listener, frequency));

            return list;
        }

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, MultipleListener<T> listener, float frequency, UnloadPool pool) {
            list.onAdd.Add(LazyListener<T>.New(listener, frequency));
            pool.Add(new UnloadAction(() => list.RemoveOnAddListener(listener)));

            return list;
        }

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action listener) {
            list.onAdd.Add(Listener<T>.New(listener));

            return list;
        }

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action listener, UnloadPool pool) {
            list.onAdd.Add(Listener<T>.New(listener));
            pool.Add(new UnloadAction(() => list.RemoveOnAddListener(listener)));

            return list;
        }

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action<T> listener) {
            list.onAdd.Add(Listener<T>.New(listener));

            return list;
        }

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action<T> listener, UnloadPool pool) {
            list.onAdd.Add(Listener<T>.New(listener));
            pool.Add(new UnloadAction(() => list.RemoveOnAddListener(listener)));

            return list;
        }

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, MultipleListener<T> listener) {
            list.onAdd.Add(Listener<T>.New(listener));

            return list;
        }

        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, MultipleListener<T> listener, UnloadPool pool) {
            list.onAdd.Add(Listener<T>.New(listener));
            pool.Add(new UnloadAction(() => list.RemoveOnAddListener(listener)));

            return list;
        }

        public static ObservedList<T> RemoveOnAddListener<T>(this ObservedList<T> list, Action listener) {
            list.onAdd.RemoveListener(listener);

            return list;
        }

        public static ObservedList<T> RemoveOnAddListener<T>(this ObservedList<T> list, Action<T> listener) {
            list.onAdd.RemoveListener(listener);

            return list;
        }

        public static ObservedList<T> RemoveOnAddListener<T>(this ObservedList<T> list, MultipleListener<T> listener) {
            list.onAdd.RemoveListener(listener);

            return list;
        }

    #endregion

    #region Remove

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action listener, float frequency) {
            list.onRemove.Add(LazyListener<T>.New(listener, frequency));

            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action listener, float frequency, UnloadPool pool) {
            list.onRemove.Add(LazyListener<T>.New(listener, frequency));
            pool.Add(new UnloadAction(() => list.RemoveOnRemoveListener(listener)));

            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action<T> listener, float frequency) {
            list.onRemove.Add(LazyListener<T>.New(listener, frequency));

            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action<T> listener, float frequency, UnloadPool pool) {
            list.onRemove.Add(LazyListener<T>.New(listener, frequency));
            pool.Add(new UnloadAction(() => list.RemoveOnRemoveListener(listener)));

            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, MultipleListener<T> listener, float frequency) {
            list.onRemove.Add(LazyListener<T>.New(listener, frequency));

            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, MultipleListener<T> listener, float frequency, UnloadPool pool) {
            list.onRemove.Add(LazyListener<T>.New(listener, frequency));
            pool.Add(new UnloadAction(() => list.RemoveOnRemoveListener(listener)));

            return list;
        }


        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action listener) {
            list.onRemove.Add(Listener<T>.New(listener));

            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action listener, UnloadPool pool) {
            list.onRemove.Add(Listener<T>.New(listener));
            pool.Add(new UnloadAction(() => list.RemoveOnRemoveListener(listener)));

            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action<T> listener) {
            list.onRemove.Add(Listener<T>.New(listener));

            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action<T> listener, UnloadPool pool) {
            list.onRemove.Add(Listener<T>.New(listener));
            pool.Add(new UnloadAction(() => list.RemoveOnRemoveListener(listener)));

            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, MultipleListener<T> listener) {
            list.onRemove.Add(Listener<T>.New(listener));

            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, MultipleListener<T> listener, UnloadPool pool) {
            list.onRemove.Add(Listener<T>.New(listener));
            pool.Add(new UnloadAction(() => list.RemoveOnRemoveListener(listener)));

            return list;
        }

        public static ObservedList<T> RemoveOnRemoveListener<T>(this ObservedList<T> list, Action listener) {
            list.onRemove.RemoveListener(listener);

            return list;
        }

        public static ObservedList<T> RemoveOnRemoveListener<T>(this ObservedList<T> list, Action<T> listener) {
            list.onRemove.RemoveListener(listener);

            return list;
        }

        public static ObservedList<T> RemoveOnRemoveListener<T>(this ObservedList<T> list, MultipleListener<T> listener) {
            list.onRemove.RemoveListener(listener);

            return list;
        }

    #endregion

    #region Clear

        public static ObservedList<T> AddOnClearListener<T>(this ObservedList<T> list, Action listener, float frequency) {
            list.onClear.Add(LazyListener.New(listener, frequency));

            return list;
        }

        public static ObservedList<T> AddOnClearListener<T>(this ObservedList<T> list, Action listener, float frequency, UnloadPool pool) {
            list.onClear.Add(LazyListener.New(listener, frequency));
            pool.Add(new UnloadAction(() => list.RemoveOnClearListener(listener)));

            return list;
        }

        public static ObservedList<T> AddOnClearListener<T>(this ObservedList<T> list, Action listener) {
            list.onClear.Add(Listener.New(listener));

            return list;
        }

        public static ObservedList<T> AddOnClearListener<T>(this ObservedList<T> list, Action listener, UnloadPool pool) {
            list.onClear.Add(Listener.New(listener));
            pool.Add(new UnloadAction(() => list.RemoveOnClearListener(listener)));

            return list;
        }

        public static ObservedList<T> RemoveOnClearListener<T>(this ObservedList<T> list, Action listener) {
            list.onClear.RemoveListener(listener);

            return list;
        }

    #endregion
    }
}