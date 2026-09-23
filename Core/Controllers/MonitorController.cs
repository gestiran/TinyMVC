// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyReactive;
using TinyReactive.Fields;

namespace TinyMVC.Controllers {
    public abstract class MonitorController<T> : IController, IApplyResolving, IBeginPlay, IUnload where T : IDependency {
        protected ObservedList<T> _models;
        
        protected readonly UnloadPool _unload;
        
        protected MonitorController() => _unload = new UnloadPool();
        
        public virtual void ApplyResolving() {
            _models = GetModels();
            
            _models.AddOnAddListener(ConnectController, _unload);
            _models.AddOnRemoveListener(DisconnectController, _unload);
        }
        
        public virtual void BeginPlay() {
            for (int modelId = 0; modelId < _models.Count; modelId++) {
                ConnectController(_models[modelId]);
            }
        }
        
        public virtual void Unload() => _unload.Unload();
        
        protected abstract ObservedList<T> GetModels();
        
        protected abstract void ConnectController(T model);
        
        protected abstract void DisconnectController(T model);
    }
    
    public class MonitorController<T1, T2> : MonitorController<T1> where T1 : IDependency where T2 : IController, IEquatable<T1>, new() {
        private readonly ObservedDependencyList<T1> _owner;
        
        public MonitorController() => ProjectContext.data.Get(out _owner);
        
        protected override ObservedList<T1> GetModels() => _owner;
        
        protected override void ConnectController(T1 model) => this.Connect<MonitorController<T1, T2>, T2>(model);
        
        protected override void DisconnectController(T1 model) => this.DisconnectReferences(model);
    }
}