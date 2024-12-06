using System.Runtime.CompilerServices;
using TinyMVC.ReactiveFields;

namespace TinyMVC.Boot.Binding {
    public abstract class Factory {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static Observed<T> Observed<T>(T value = default) => new Observed<T>(value);
    }
}