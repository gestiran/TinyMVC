using System;

namespace TinyMVC.Loop {
    public sealed class UnloadAction : IUnload {
        private readonly Action _action;
        
        internal UnloadAction(Action action) => _action = action;
        
        public static implicit operator UnloadAction(Action action) => new UnloadAction(action);
        
        public void Unload() => _action.Invoke();
    }
}