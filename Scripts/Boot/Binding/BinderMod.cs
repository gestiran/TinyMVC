// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Dependencies;
using TinyReactive;

namespace TinyMVC.Boot.Binding {
    public abstract class BinderMod {
        protected UnloadPool _unload { get; private set; }
        
        internal abstract void BindInternal();
        
        internal void ConnectUnload(UnloadPool unload) => _unload = unload;
    }
    
    public abstract class BinderMod<T> : BinderMod where T : IDependency {
        internal override void BindInternal() {
            if (ProjectContext.data.Get(out T model)) {
                Bind(model);
            }
        }
        
        protected abstract void Bind(T model);
    }
}