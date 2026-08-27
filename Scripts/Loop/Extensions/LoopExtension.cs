// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyMVC.Boot.Contexts;

namespace TinyMVC.Loop.Extensions {
    internal static class LoopExtension {
        internal static void CheckAndAdd<T>(this ControllersContext context, List<T> collection) where T : ILoop {
            for (int systemId = 0; systemId < context.systems.Count; systemId++) {
                if (context.systems[systemId] is T controller) {
                    collection.Add(controller);
                }
            }
        }
    }
}