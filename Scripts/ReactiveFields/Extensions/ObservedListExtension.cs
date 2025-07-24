// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ObservedListExtension {
        public static T[] ToArray<T>(this ObservedList<T> list) {
            T[] result = new T[list.count];
            
            for (int i = 0; i < result.Length; i++) {
                result[i] = list[i];
            }
            
            return result;
        }
    }
}