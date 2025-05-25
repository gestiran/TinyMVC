using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;

namespace TinyMVC.Boot.Contexts {
    public abstract class ControllersContext {
        [ShowInInspector] 
        internal readonly List<IController> systems;
        
        [ShowInInspector, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Group", ValueLabel = "Controllers")] 
        private readonly Dictionary<string, List<IController>> _controllers;
        
        private Action _lazyInit;
        
        public sealed class EmptyContext : ControllersContext {
            internal EmptyContext() { }
            
            protected override void Create() { }
        }
        
        protected ControllersContext() {
            systems = new List<IController>();
            _controllers = new Dictionary<string, List<IController>>();
        }
        
        public static EmptyContext Empty() => new EmptyContext();
        
        internal void CreateControllers() => Create();
        
        internal void Init() {
            _lazyInit?.Invoke();
            _lazyInit = null;
        }
        
        internal async UniTask InitAsync() => await systems.TryInitAsync();
        
        internal async UniTask BeginPlay() => await systems.TryBeginPlayAsync();
        
        internal void CheckAndAdd<T>(List<T> list) {
            for (int systemId = 0; systemId < systems.Count; systemId++) {
                if (systems[systemId] is T controller) {
                    list.Add(controller);
                }
            }
        }
        
        internal void Connect<T1, T2>(T2 system, T1 controller, Action<ILoop> connectLoop, Action<IResolving> resolve) where T1 : IController where T2 : IController {
            if (controller is IInit init) {
                init.Init();
            }
            
            if (controller is IResolving resolving) {
                resolve(resolving);
                
                if (controller is IApplyResolving apply) {
                    apply.ApplyResolving();
                }
            }
            
            if (controller is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }
            
            if (controller is ILoop loop) {
                connectLoop(loop);
            }
            
            string systemName = system.GetType().Name;
            
            if (_controllers.TryGetValue(systemName, out List<IController> controllers)) {
                controllers.Add(controller);
            } else {
                _controllers.Add(systemName, new List<IController>() { controller });
            }
        }
        
        internal void Disconnect<T1, T2>(T2 system, T1 controller, Action<ILoop> disconnectLoop) where T1 : IController where T2 : IController {
            if (controller is ILoop loop) {
                disconnectLoop(loop);
            }
            
            if (controller is IUnload unload) {
                unload.Unload();
            }
            
            if (_controllers.TryGetValue(system.GetType().Name, out List<IController> controllers)) {
                if (_controllers.TryGetValue(controller.GetType().Name, out List<IController> subControllers)) {
                    for (int controllerId = 0; controllerId < subControllers.Count; controllerId++) {
                        DisconnectNR(controller, subControllers[controllerId], disconnectLoop);
                    }
                }
                
                controllers.Remove(controller);
            }
        }
        
        internal IEnumerable<IController> ForEach(string systemName) {
            if (_controllers.TryGetValue(systemName, out List<IController> controllers)) {
                for (int controllerId = controllers.Count - 1; controllerId >= 0; controllerId--) {
                    yield return controllers[controllerId];
                }
            }
        }
        
        internal IEnumerable<T> ForEach<T>(string systemName) where T : IController {
            if (_controllers.TryGetValue(systemName, out List<IController> controllers)) {
                for (int controllerId = controllers.Count - 1; controllerId >= 0; controllerId--) {
                    if (controllers[controllerId] is T controller) {
                        yield return controller;
                    }
                }
            }
        }
        
        internal void Unload() {
            foreach (List<IController> controllers in _controllers.Values) {
                controllers.TryUnload();
            }
            
            systems.TryUnload();
        }
        
        protected void Add<T>() where T : IController, new() => _lazyInit += () => systems.Add(new T());
        
        private void DisconnectNR<T1, T2>(T2 system, T1 controller, Action<ILoop> disconnectLoop) where T1 : IController where T2 : IController {
            Disconnect(system, controller, disconnectLoop);
        }
        
        protected abstract void Create();
    }
}