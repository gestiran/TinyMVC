// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;

namespace TinyMVC.Boot.Helpers {
    internal static class ContextExtensions {
        internal static bool TryGetContext<T1, T2>(this List<T1> list, string contextKey, out T2 context, out int id) where T1 : ContextLink<T2> {
            for (int i = 0; i < list.Count; i++) {
                if (list[i].contextKey != contextKey) {
                    continue;
                }
                
                context = list[i].context;
                id = i;
                
                return true;
            }
            
            context = default;
            id = default;
            
            return false;
        }
    }
}