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
        
        Dictionary<IController, UnloadPool> IContext.unloads => _unloads;
        IViewsContext IContext.views => _views;
        IContextModule[] IContext.modules => _modules;
        UnloadPool IContext.unloadPool => _unload;
        
        ModelsContext IContext.models { get; set; }
        ParametersContext IContext.parameters { get; set; }
        
        ControllersContext IContext.controllers { get => _controllers; set => _controllers = value; }
        int IContext.id { get => _id; set => _id = value; }
        
        private ControllersContext _controllers;
        private int _id;
        private CancellationTokenSource _cancellationSource;
        
        private readonly UnloadPool _unload;
        private readonly IViewsContext _views;
        private readonly IContextModule[] _modules;
        private readonly Dictionary<IController, UnloadPool> _unloads;
        private readonly TaskCompletionSource<bool> _initializationStatus;
        
        protected Context() {
            _views = CreateViews();
            _modules = CreateModules() ?? Array.Empty<IContextModule>();
            _unloads = new Dictionary<IController, UnloadPool>();
            _unload = new UnloadPool();
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
        
        void IContext.Create() => this.Create();
        
        async Task IContext.InitAsync() {
            try {
                await this.InitAsync();
                OnInitializationComplete();
            } catch (Exception exception) {
                DebugUtility.LogException(exception);
            }
            
            _initializationStatus.TrySetResult(true);
        }
        
        Task IContext.Remove() => RemoveAsync();
        
        void IContext.Connect<T1, T2>(T2 system, T1 controller) => ConnectController(system, controller);
        
        void IContext.Disconnect<T1, T2>(T2 system, T1 controller) => DisconnectController(system, controller);
        
        ControllersContext IContext.CreateControllers() => CreateControllers();
        
        ModelsContext IContext.CreateModels() => CreateModels();
        
        ParametersContext IContext.CreateParameters() => CreateParameters();
        
        internal virtual IViewsContext CreateViews() => new EmptyViews();
        
        internal virtual IContextModule[] CreateModules() => Array.Empty<IContextModule>();
        
        protected virtual void OnInitializationComplete() { }
        
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
            
            try {
                OnControllerConnected(controller);
            } catch (Exception exception) {
                DebugUtility.LogException(exception);
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
            try {
                OnControllerDisconnected(controller);
            } catch (Exception exception) {
                DebugUtility.LogException(exception);
            }
            
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
        
        protected virtual void OnControllerConnected(IController controller) { }
        
        protected virtual void OnControllerDisconnected(IController controller) { }
        
        public bool Equals(Context other) => other != null && key == other.key;
        
        public override bool Equals(object obj) => obj is Context other && Equals(other);
        
        public override int GetHashCode() => key.GetHashCode();
        
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