// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyUtilities.Logger;

namespace TinyMVC.Loop.Extensions {
    public static class TickExtension {
        public static void Tick<T>(this ICollection<T> collection) where T : ITick {
            foreach (T obj in collection) {
                try {
                    obj.Tick();
                } catch (Exception exception) {
                    DebugUtility.LogException(exception);
                }
            }
        }
    }
}