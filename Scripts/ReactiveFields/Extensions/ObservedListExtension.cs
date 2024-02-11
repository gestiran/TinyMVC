using System;
using TinyMVC.Loop;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ObservedListExtension {
        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action listener) {
            list.onAdd.Add(listener);
            return list;
        }
        
        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action listener, UnloadPool pool) {
            list.onAdd.Add(listener);
            pool.Add(new UnloadAction(() => list.RemoveOnAddListener(listener)));
            return list;
        }
        
        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action<T> listener) {
            list.onAdd.Add(listener);
            return list;
        }
        
        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, Action<T> listener, UnloadPool pool) {
            list.onAdd.Add(listener);
            pool.Add(new UnloadAction(() => list.RemoveOnAddListener(listener)));
            return list;
        }
        
        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, MultipleListener<T> listener) {
            list.onAdd.Add(listener);
            return list;
        }
        
        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, MultipleListener<T> listener, UnloadPool pool) {
            list.onAdd.Add(listener);
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
        
        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action listener) {
            list.onRemove.Add(listener);
            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action listener, UnloadPool pool) {
            list.onRemove.Add(listener);
            pool.Add(new UnloadAction(() => list.RemoveOnRemoveListener(listener)));
            return list;
        }
        
        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action<T> listener) {
            list.onRemove.Add(listener);
            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, Action<T> listener, UnloadPool pool) {
            list.onRemove.Add(listener);
            pool.Add(new UnloadAction(() => list.RemoveOnRemoveListener(listener)));
            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, MultipleListener<T> listener) {
            list.onRemove.Add(listener);
            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, MultipleListener<T> listener, UnloadPool pool) {
            list.onRemove.Add(listener);
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

        public static ObservedList<T> AddOnClearListener<T>(this ObservedList<T> list, Action listener) {
            list.onClear.Add(listener);
            return list;
        }
        
        public static ObservedList<T> AddOnClearListener<T>(this ObservedList<T> list, Action listener, UnloadPool pool) {
            list.onClear.Add(listener);
            pool.Add(new UnloadAction(() => list.RemoveOnClearListener(listener)));
            return list;
        }

        public static ObservedList<T> RemoveOnClearListener<T>(this ObservedList<T> list, Action listener) {
            list.onClear.RemoveListener(listener);
            return list;
        }
    }
}