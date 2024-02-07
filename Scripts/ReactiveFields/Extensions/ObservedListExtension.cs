using System;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ObservedListExtension {
        public static ObservedList<T> AddOnAddListener<T>(this ObservedList<T> list, MultipleListener<T> listener) {
            list.onAdd.Add(listener);
            return list;
        }

        public static ObservedList<T> RemoveOnAddListener<T>(this ObservedList<T> list, MultipleListener<T> listener) {
            list.onAdd.Remove(listener);
            return list;
        }

        public static ObservedList<T> AddOnRemoveListener<T>(this ObservedList<T> list, MultipleListener<T> listener) {
            list.onRemove.Add(listener);
            return list;
        }

        public static ObservedList<T> RemoveOnRemoveListener<T>(this ObservedList<T> list, MultipleListener<T> listener) {
            list.onRemove.Remove(listener);
            return list;
        }

        public static ObservedList<T> AddOnClearListener<T>(this ObservedList<T> list, Action listener) {
            list.onClear.Add(listener);
            return list;
        }

        public static ObservedList<T> RemoveOnClearListener<T>(this ObservedList<T> list, Action listener) {
            list.onClear.Remove(listener);
            return list;
        }
    }
}