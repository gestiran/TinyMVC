// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    public abstract class DependencyPoolBinder<T> : DependencyAbstractPoolBinder<T> where T : IDependency, new() {
        protected DependencyPoolBinder(string key = null) => keyValue = key;
        
        protected override T New(int index) => new T();
    }
}