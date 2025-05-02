using System;

namespace TinyMVC.Loop {
    public sealed class UnloadAction : IUnload {
        private readonly Action _action;
        
        public UnloadAction(Action action) => _action = action;
        
        public void Unload() => _action.Invoke();
    }
}