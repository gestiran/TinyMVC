using System.Collections.Generic;

namespace TinyMVC.ReactiveFields {
    internal static class Listeners {
        internal static readonly Dictionary<int, List<ActionListener>> pool;
        
        static Listeners() => pool = new Dictionary<int, List<ActionListener>>(4096);
    }
    
    internal static class Listeners<T> {
        internal static readonly Dictionary<int, List<ActionListener<T>>> pool;
        
        static Listeners() => pool = new Dictionary<int, List<ActionListener<T>>>(128);
    }
}