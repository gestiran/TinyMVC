// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;
using TinyReactive;
using TinyReactive.Extensions;
using TinyReactive.Fields;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot.Contexts {
    public abstract class ControllersContext : IController {
        protected UnloadPool _unload { get; private set; }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector]
    #endif
        internal readonly List<IController> systems;
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Group", ValueLabel = "Controllers")]
    #endif
        private readonly Dictionary<string, List<IController>> _controllers;
        
        internal List<ActionListener> initLazyList;
        
        private static EmptyContext _empty;
        
        public sealed class EmptyContext : ControllersContext {
            internal EmptyContext() { }
            
            protected override void Create() { }
        }
        
        protected ControllersContext() {
            systems = new List<IController>();
            _controllers = new Dictionary<string, List<IController>>();
            initLazyList = new List<ActionListener>();
        }
        
        public static EmptyContext Empty() {
            if (_empty == null) {
                _empty = new EmptyContext();
            }
            
            return _empty;
        }
        
        internal void ConnectUnload(UnloadPool unload) => _unload = unload;
        
        internal void CreateControllers() => Create();
        
        internal void Init() {
            foreach (ActionListener listener in initLazyList) {
                listener.Invoke();
            }
            
            initLazyList = null;
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
        
        internal void Connect<T1, T2>(T2 system, T1 controller, Action<ILoop> connectLoop) where T1 : IController where T2 : IController {
            if (controller is IInit init) {
                init.Init();
            }
            
            if (controller is IApplyResolving apply) {
                apply.ApplyResolving();
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
        
        protected void Add<T>() where T : IController, new() => initLazyList.Add(() => systems.Add(new T()));
        
        private void DisconnectNR<T1, T2>(T2 system, T1 controller, Action<ILoop> disconnectLoop) where T1 : IController where T2 : IController {
            Disconnect(system, controller, disconnectLoop);
        }
        
        protected abstract void Create();
    }
}