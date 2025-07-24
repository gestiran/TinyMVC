// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyMVC.Dependencies {
    public sealed class Dependency : IDependency {
        internal readonly IDependency link;
        internal readonly Type[] types;
        
        internal Dependency(IDependency link, params Type[] types) {
            this.link = link;
            this.types = types;
        }
    }
}