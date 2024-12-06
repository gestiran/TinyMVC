using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;

namespace TinyMVC.Boot.Contexts {
    public abstract class ControllersContext {
        [ShowInInspector] 
        private readonly List<IController> _systems;
        
        [ShowInInspector, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Group", ValueLabel = "Controllers")] 
        private readonly Dictionary<string, List<IController>> _controllers;
        
        public sealed class EmptyContext : ControllersContext {
            internal EmptyContext() { }
            
            protected override void Create() { }
        }
        
        protected ControllersContext() {
            _systems = new List<IController>();
            _controllers = new Dictionary<string, List<IController>>();
        }
        
        public static EmptyContext Empty() => new EmptyContext();
        
        internal void CreateControllers() => Create();
        
        internal async Task InitAsync() => await _systems.TryInitAsync();
        
        internal async Task BeginPlay() => await _systems.TryBeginPlayAsync();
        
        internal void CheckAndAdd<T>(List<T> list) {
            list.Capacity += _systems.Count;
            
            for (int systemId = 0; systemId < _systems.Count; systemId++) {
                if (_systems[systemId] is T controller) {
                    list.Add(controller);
                }
            }
        }
        
        internal void Connect<T1, T2>(T2 system, T1 controller, int sceneId, Action<IResolving> resolve) where T1 : IController where T2 : IController {
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
                ProjectContext.ConnectLoop(sceneId, loop);
            }
            
            Type systemType = system.GetType();
            
            if (_controllers.TryGetValue(systemType.Name, out List<IController> controllers)) {
                controllers.Add(controller);
            } else {
                _controllers.Add(systemType.Name, new List<IController>() { controller });
            }
        }
        
        internal void Disconnect<T1, T2>(T2 system, T1 controller, int sceneId) where T1 : IController where T2 : IController {
            if (controller is ILoop loop) {
                ProjectContext.DisconnectLoop(sceneId, loop);
            }
            
            if (controller is IUnload unload) {
                unload.Unload();
            }
            
            if (_controllers.TryGetValue(system.GetType().Name, out List<IController> controllers)) {
                controllers.Remove(controller);
            }
        }
        
        internal void Unload() {
            foreach (List<IController> controllers in _controllers.Values) {
                controllers.TryUnload();
            }
            
            _systems.TryUnload();
        }
        
        protected void Add<T>() where T : IController, new() => _systems.Add(new T());
        
        protected abstract void Create();
    }
}