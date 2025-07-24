// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;

namespace TinyMVC.Samples.Models.Global {
    public sealed class EventSystemModel : IDependency {
        public Observed<bool> isActive;
    }
}