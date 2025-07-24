// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using TinyMVC.Boot.Contexts;

namespace TinyMVC.Boot.Empty {
    [Serializable]
    public sealed class ViewsEmptyContext : ViewsContext {
        protected override void Create() { }
    }
}