using System.Collections.Generic;

namespace TinyMVC.Loop {
    public sealed class UnloadPool {
        private readonly List<IUnload> _unloads;

        public UnloadPool(int capacity = 4) => _unloads = new List<IUnload>(capacity);

        public void Add(IUnload unload) => _unloads.Add(unload);

        public void Remove(IUnload unload) => _unloads.Remove(unload);

        public void Clear() => _unloads.Clear();

        public void Unload() {
            for (int i = 0; i < _unloads.Count; i++) {
                _unloads[i].Unload();
            }
        }
    }

    public sealed class UnloadPool<T> where T : IUnload {
        private readonly List<T> _unloads;

        public UnloadPool(int capacity = 4) => _unloads = new List<T>(capacity);

        public void Add(T unload) => _unloads.Add(unload);

        public void Remove(T unload) => _unloads.Remove(unload);

        public void Clear() => _unloads.Clear();

        public void Unload() {
            for (int i = 0; i < _unloads.Count; i++) {
                _unloads[i].Unload();
            }
        }
    }
}