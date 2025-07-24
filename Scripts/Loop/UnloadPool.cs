// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using JetBrains.Annotations;

namespace TinyMVC.Loop {
    public sealed class UnloadPool : IUnload {
        public bool isUnloaded { get; private set; }
        
        private readonly List<IUnload> _unloads;
        
        public UnloadPool(int capacity = 4) => _unloads = new List<IUnload>(capacity);
        
        public void Add([NotNull] IUnload unload) => _unloads.Add(unload);
        
        public void Add([NotNull] params IUnload[] unloads) => _unloads.AddRange(unloads);
        
        public void Remove([NotNull] IUnload unload) => _unloads.Remove(unload);
        
        public void Remove([NotNull] params IUnload[] unloads) {
            foreach (IUnload unload in unloads) {
                _unloads.Remove(unload);
            }
        }
        
        public void Clear() => _unloads.Clear();
        
        public void Unload() {
            foreach (IUnload unload in _unloads) {
                unload.Unload();
            }
            
            isUnloaded = true;
        }
    }
    
    public sealed class UnloadPool<T> : IUnload where T : IUnload {
        private readonly List<T> _unloads;
        
        public UnloadPool(int capacity = 4) => _unloads = new List<T>(capacity);
        
        public void Add([NotNull] T unload) => _unloads.Add(unload);
        
        public void Add([NotNull] params T[] unloads) => _unloads.AddRange(unloads);
        
        public void Remove([NotNull] T unload) => _unloads.Remove(unload);
        
        public void Remove([NotNull] params T[] unloads) {
            foreach (T unload in unloads) {
                _unloads.Remove(unload);
            }
        }
        
        public void Clear() => _unloads.Clear();
        
        public void Unload() {
            foreach (T unload in _unloads) {
                unload.Unload();
            }
        }
    }
}