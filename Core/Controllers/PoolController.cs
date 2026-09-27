// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyReactive;

namespace TinyMVC.Controllers {
    public abstract class PoolController : IController, IUnload {
        protected readonly UnloadPool _unload;
        
        protected PoolController() => _unload = new UnloadPool();
        
        public virtual void Unload() => _unload.Unload();
    }
    
    public abstract class PoolController<T> : PoolController, IApplyResolving, IBeginPlay where T : IDependency {
        protected IEnumerable<T> _models;
        
        public virtual void ApplyResolving() => _models = GetModels();
        
        public virtual void BeginPlay() {
            foreach (T model in _models) {
                ConnectController(model);
            }
        }
        
        protected abstract IEnumerable<T> GetModels();
        
        protected abstract void ConnectController(T model);
    }
    
    public class PoolController<T1, T2> : PoolController<T1> where T1 : IDependency where T2 : IController, IEquatable<T1>, new() {
        private readonly DependencyPool<T1> _owner;
        
        public PoolController() => ProjectContext.data.Get(out _owner);
        
        protected override IEnumerable<T1> GetModels() => _owner;
        
        protected override void ConnectController(T1 model) => this.Connect<PoolController<T1, T2>, T2>(model);
    }
}