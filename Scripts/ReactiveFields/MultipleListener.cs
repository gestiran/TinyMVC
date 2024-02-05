using System.Diagnostics.CodeAnalysis;

namespace TinyMVC.ReactiveFields {
    public delegate void MultipleListener<in T>([NotNull] params T[] values);
}