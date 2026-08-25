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
using TinyUtilities.Logger;

namespace TinyMVC.Boot {
    public abstract class Context : IContext, IEquatable<Context> {
        public virtual string key => GetType().Name;
        
        public CancellationToken cancellation => cancellationSource?.Token ?? CancellationToken.None;
        
        public ControllersContext controllers { get; set; }
        
        Dictionary<IController, UnloadPool> IContext.unloads => _unloads;
        IViewsContext IContext.views => _views;
        IContextModule[] IContext.modules => _modules;
        UnloadPool IContext.unloadPool => unloadPool;
        Task IContext.initialization => _initCompletionSource.Task;
        
        ControllersContext IContext.controllers { get => controllers; set => controllers = value; }
        ModelsContext IContext.models { get => models; set => models = value; }
        ParametersContext IContext.parameters { get => parameters; set => parameters = value; }
        int IContext.sceneId { get => _sceneId; set => _sceneId = value; }
        CancellationTokenSource IContext.cancellationSource { get => cancellationSource; set => cancellationSource = value; }
        bool IContext.isInitializationComplete { get => _isInitializationComplete; set => _isInitializationComplete = value; }
        
        internal ModelsContext models { get; set; }
        internal ParametersContext parameters { get; set; }
        
        internal CancellationTokenSource cancellationSource;
        
        private int _sceneId;
        private bool _isInitializationComplete;
        private bool _isInitialized;
        private bool _isRemoved;
        
        private readonly UnloadPool unloadPool;
        private readonly IViewsContext _views;
        private readonly IContextModule[] _modules;
        private readonly Dictionary<IController, UnloadPool> _unloads;
        private readonly TaskCompletionSource<bool> _initCompletionSource;
        
        protected Context() {
            _views = CreateViews();
            _modules = CreateModules() ?? Array.Empty<IContextModule>();
            _unloads = new Dictionary<IController, UnloadPool>();
            unloadPool = new UnloadPool();
            _initCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
        
        public async Task Initialize() {
            if (_isInitialized) {
                return;
            }
            
            _isInitialized = true;
            await ProjectContext.AddContext(this, _sceneId);
            await _initCompletionSource.Task;
        }
        
        public async Task RemoveAsync() {
            if (_isRemoved) {
                return;
            }
            
            _isRemoved = true;
            
            if (!_isInitialized) {
                return;
            }
            
            try {
                await this.Remove();
            } catch (Exception exception) {
                DebugUtility.LogException(exception);
            }
        }
        
        public T Add<T>(T unload) where T : IUnload {
            if (unloadPool == null) {
                return unload;
            }
            
            return unloadPool.Add(unload);
        }
        
        void IContext.Create() => this.Create();
        
        async Task IContext.InitAsync() {
            try {
                await this.InitAsync();
                OnInitializationComplete();
            } catch (Exception exception) {
                DebugUtility.LogException(exception);
            }
            
            _isInitializationComplete = true;
            _initCompletionSource.TrySetResult(true);
        }
        
        async Task IContext.Remove() {
            if (_isRemoved) {
                return;
            }
            
            _isRemoved = true;
            
            try {
                await this.Remove();
            } catch (Exception exception) {
                DebugUtility.LogException(exception);
            }
        }
        
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
            
            if (controllers == null) {
                return;
            }
            
            string systemName = system.GetType().Name;
            
            if (controllers.controllers.TryGetValue(systemName, out List<IController> list)) {
                list.Add(controller);
            } else {
                controllers.controllers.Add(systemName, new List<IController> { controller });
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
            
            if (controllers == null) {
                return;
            }
            
            string systemName = system.GetType().Name;
            
            if (controllers.controllers.TryGetValue(controller.GetType().Name, out List<IController> subControllers)) {
                for (int controllerId = subControllers.Count - 1; controllerId >= 0; controllerId--) {
                    DisconnectController(system, subControllers[controllerId]);
                }
            }
            
            if (controllers.controllers.TryGetValue(systemName, out List<IController> list)) {
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