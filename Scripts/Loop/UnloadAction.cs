// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyMVC.Loop {
    public sealed class UnloadAction : IUnload {
        private readonly Action _action;
        
        public UnloadAction(Action action) => _action = action;
        
        public void Unload() => _action.Invoke();
    }
}