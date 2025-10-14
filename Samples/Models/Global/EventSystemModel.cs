// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Dependencies;
using TinyReactive.Fields;

namespace TinyMVC.Samples.Models.Global {
    public class EventSystemModel : IDependency {
        public Observed<bool> isActive;
    }
}