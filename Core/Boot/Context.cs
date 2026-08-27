// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TinyMVC.Boot.Contexts;
using TinyMVC.Boot.Extensions;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Views;
using TinyReactive;
using TinyUtilities.Extensions;
using TinyUtilities.Logger;

namespace TinyMVC.Boot {
    public abstract class Context : IContext, IEquatable<Context> {
        public virtual string key => GetType().Name;
        
        public CancellationToken cancellation => _cancellationSource?.Token ?? CancellationToken.None;
        
        ControllersContext IContext.controllers => _controllers;
        ModelsContext IContext.models => _models;
        ParametersContext IContext.parameters => _parameters;
        IViewsContext IContext.views => _views;
        UnloadPool IContext.unloadPool => _unload;
        Dictionary<IController, UnloadPool> IContext.unloads => _unloads;
        
        private ControllersContext _controllers;
        private ModelsContext _models;
        private ParametersContext _parameters;
        private CancellationTokenSource _cancellationSource;
        
        private readonly int _id;
        private readonly UnloadPool _unload;
        private readonly Dictionary<IController, UnloadPool> _unloads;
        private readonly IViewsContext _views;
        private readonly List<ContextModule> _modules;
        private readonly TaskCompletionSource<bool> _initializationStatus;
        
        protected Context() {
            _id = GetType().Name.GetHashCode();
            _unload = new UnloadPool();
            _unloads = new Dictionary<IController, UnloadPool>();
            _modules = new List<ContextModule>();
            _views = CreateViews();
            _initializationStatus = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
        
        public async Task Initialize() {
            await ProjectContext.AddContext(this, _id);
            
            if (_initializationStatus.Task.IsCompleted == false) {
                DebugUtility.LogError($"Context.Initialize: context '{key}' was not registered (duplicate key?), initialization skipped.");
                _initializationStatus.TrySetResult(true);
            }
            
            await _initializationStatus.Task;
        }
        
        void IContext.Create() {
            _controllers = CreateControllers();
            _models = CreateModels();
            _parameters = CreateParameters();
            
            CreateModules();
            
            this.Create();
        }
        
        async Task IContext.InitAsync() {
            try {
                await this.InitAsync();
            } catch (Exception exception) {
                DebugUtility.LogException(exception);
            }
            
            _initializationStatus.TrySetResult(true);
        }
        
        IEnumerable<IContextComponent> IContext.Components() {
            for (int componentId = 0; componentId < _modules.Count; componentId++) {
                yield return _modules[componentId];
            }
        }
        
        public async Task RemoveAsync() {
            await _initializationStatus.Task;
            
            try {
                _cancellationSource = _cancellationSource.Reset();
                
                ProjectContext.RemoveContext(this, _id);
            } catch (Exception exception) {
                DebugUtility.LogException(exception);
            }
        }
        
        public T Add<T>(T unload) where T : IUnload {
            if (_unload == null) {
                return unload;
            }
            
            return _unload.Add(unload);
        }
        
        void IContext.Connect<T1, T2>(T2 system, T1 controller) => ConnectController(system, controller);
        
        void IContext.Disconnect<T1, T2>(T2 system, T1 controller) => DisconnectController(system, controller);
        
        internal virtual IViewsContext CreateViews() => new EmptyViews();
        
        protected virtual void CreateModules() { }
        
        protected void AddModule<T>(T module) where T : ContextModule => _modules.Add(module);
        
        protected abstract ControllersContext CreateControllers();
        
        protected abstract ModelsContext CreateModels();
        
        protected abstract ParametersContext CreateParameters();
        
        protected virtual void ConnectController<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController {
            if (controller is IInit init) {
                try {
                    init.Init();
                } catch (Exception exception) {
                    DebugUtility.LogException(exception);
                }
            }
            
            if (controller is IApplyResolving applyResolving) {
                try {
                    applyResolving.ApplyResolving();
                } catch (Exception exception) {
                    DebugUtility.LogException(exception);
                }
            }
            
            if (controller is IBeginPlay beginPlay) {
                try {
                    beginPlay.BeginPlay();
                } catch (Exception exception) {
                    DebugUtility.LogException(exception);
                }
            }
            
            if (_controllers == null) {
                return;
            }
            
            string systemName = system.GetType().Name;
            
            if (_controllers.controllers.TryGetValue(systemName, out List<IController> list)) {
                list.Add(controller);
            } else {
                _controllers.controllers.Add(systemName, new List<IController> { controller });
            }
        }
        
        protected virtual void DisconnectController<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController {
            if (controller is IUnload unload) {
                try {
                    unload.Unload();
                } catch (Exception exception) {
                    DebugUtility.LogException(exception);
                }
            }
            
            if (_unloads.Remove(controller, out UnloadPool globalUnload)) {
                try {
                    globalUnload.Unload();
                } catch (Exception exception) {
                    DebugUtility.LogException(exception);
                }
            }
            
            if (_controllers == null) {
                return;
            }
            
            string systemName = system.GetType().Name;
            
            if (_controllers.controllers.TryGetValue(controller.GetType().Name, out List<IController> subControllers)) {
                for (int controllerId = subControllers.Count - 1; controllerId >= 0; controllerId--) {
                    DisconnectController(system, subControllers[controllerId]);
                }
            }
            
            if (_controllers.controllers.TryGetValue(systemName, out List<IController> list)) {
                list.Remove(controller);
            }
        }
        
        Task IContext.Remove() => RemoveAsync();
        
        public bool Equals(Context other) => other != null && _id == other._id;
        
        public override bool Equals(object obj) => obj is Context other && Equals(other);
        
        public override int GetHashCode() => _id;
        
        public sealed class EmptyViews : IViewsContext {
            void IViewsContext.Instantiate() { }
            
            void IViewsContext.GetDependencies(List<IDependency> dependencies) { }
            
            void IViewsContext.CreateViews() { }
            
            Task IViewsContext.InitAsync() => Task.CompletedTask;
            
            Task IViewsContext.BeginPlay() => Task.CompletedTask;
            
            void IViewsContext.Unload() { }
            
            void IViewsContext.AddView(IView view) { }
            
            void IViewsContext.TryApplyResolving() { }
        }
    }
}