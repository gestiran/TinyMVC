// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Controllers;
using TinyMVC.Loop.Extensions;
using TinyReactive;
using TinyReactive.Fields;

namespace TinyMVC.Boot.Contexts {
    public abstract class ControllersContext : IController {
        protected UnloadPool _unload { get; private set; }
        
        internal List<ActionListener> initLazyList { get; private set; }
        
        internal readonly List<IController> systems;
        internal readonly Dictionary<string, List<IController>> controllers;
        
        private static EmptyContext _empty;
        
        public sealed class EmptyContext : ControllersContext {
            internal EmptyContext() { }
            
            protected override void Create() { }
        }
        
        protected ControllersContext() {
            systems = new List<IController>();
            controllers = new Dictionary<string, List<IController>>();
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
        
        public void Init() {
            if (initLazyList == null) {
                return;
            }
            
            foreach (ActionListener listener in initLazyList) {
                listener.Invoke();
            }
            
            initLazyList = null;
        }
        
        internal async Task InitAsync() => await systems.TryInitAsync();
        
        internal async Task BeginPlay() => await systems.TryBeginPlayAsync();
        
        internal IEnumerable<IController> ForEach(string systemName) {
            if (controllers.TryGetValue(systemName, out List<IController> systemControllers)) {
                for (int controllerId = systemControllers.Count - 1; controllerId >= 0; controllerId--) {
                    yield return systemControllers[controllerId];
                }
            }
        }
        
        internal IEnumerable<T> ForEach<T>(string systemName) where T : IController {
            if (controllers.TryGetValue(systemName, out List<IController> systemControllers)) {
                for (int controllerId = systemControllers.Count - 1; controllerId >= 0; controllerId--) {
                    if (systemControllers[controllerId] is T controller) {
                        yield return controller;
                    }
                }
            }
        }
        
        internal void Unload() {
            foreach (List<IController> contextControllers in controllers.Values) {
                contextControllers.TryUnload();
            }
            
            systems.TryUnload();
        }
        
        protected void Add<T>() where T : IController, new() => initLazyList.Add(() => systems.Add(new T()));
        
        protected abstract void Create();
    }
}