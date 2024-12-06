using System;
using System.Collections.Generic;

namespace TinyMVC.ReactiveFields {
    internal static class Listeners {
        internal static readonly Dictionary<int, List<Action>> pool;
        
        static Listeners() => pool = new Dictionary<int, List<Action>>();
    }
    
    internal static class Listeners<T> {
        internal static readonly Dictionary<int, List<Action<T>>> pool;
        
        static Listeners() => pool = new Dictionary<int, List<Action<T>>>();
    }
}