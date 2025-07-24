// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Modules.Networks.Extensions {
    internal static class ArrayExtension {
        public static bool EqualsValues(this byte[] first, byte[] second) {
            if (first.Length != second.Length) {
                return false;
            }
            
            for (int groupId = 0; groupId < second.Length; groupId++) {
                if (first[groupId] != second[groupId]) {
                    return false;
                }
            }
            
            return true;
        }
    }
}