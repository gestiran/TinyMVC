// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TinyMVC.Controllers;
using TinyMVC.Boot.Contexts;
using TinyReactive;

namespace TinyMVC.Boot {
    public interface IContext : IUnloadLink {
        /// <summary> Unique context identifier. Unity: the name of the context GameObject. </summary>
        public string key { get; }
        
        /// <summary> Cancellation token of the context lifetime. </summary>
        public CancellationToken cancellation { get; }
        
        /// <summary> Per-controller unload pools. </summary>
        internal Dictionary<IController, UnloadPool> unloads { get; }
        
        /// <summary> Controllers composition of the context. Filled during <see cref="Create"/>. </summary>
        internal ControllersContext controllers { get; }
        
        /// <summary> Models composition of the context. Filled during <see cref="Create"/>. </summary>
        internal ModelsContext models { get; }
        
        /// <summary> Parameters composition of the context. Filled during <see cref="Create"/>. </summary>
        internal ParametersContext parameters { get; }
        
        /// <summary> Views composition of the context. </summary>
        internal IViewsContext views { get; }
        
        /// <summary> Modules attached to the context. </summary>
        internal IContextModule[] modules { get; }
        
        /// <summary> Pool unloaded with the current context. </summary>
        internal UnloadPool unloadPool { get; }
        
        /// <summary> Creates all context sub-systems: controllers, models, parameters, views and modules. </summary>
        internal void Create();
        
        /// <summary> Runs the full initialization sequence: view init, resolve, begin play and loop registration. </summary>
        internal Task InitAsync();
        
        /// <summary> Waits for initialization completion, then unregisters the context from the project. </summary>
        internal Task Remove();
        
        /// <summary> Connects a runtime-created controller to the system of another controller. </summary>
        internal void Connect<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController;
        
        /// <summary> Disconnects a runtime-created controller from the system of another controller. </summary>
        internal void Disconnect<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController;
    }
}