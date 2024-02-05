using System.Diagnostics.CodeAnalysis;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ObservedListExtension {
        public static void AddRange<T>(this ObservedList<T> list, [NotNull] params T[] array) {
            for (int i = 0; i < array.Length; i++) {
                list.Add(array[i]);
            }
        }
    }
}