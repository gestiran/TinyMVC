// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    internal interface IBinder {
        public IDependency GetDependency();
        
        public Binder current { get; }
    }
}