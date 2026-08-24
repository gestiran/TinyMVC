// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TinyMVC.Controllers;
using TinyMVC.Boot.Contexts;
using TinyMVC.Boot.Extensions;
using TinyReactive;

namespace TinyMVC.Boot {
    /// <summary>
    /// Single contract of the context.<br/>
    /// Combines common operations executed by <see cref="ContextExtension">ContextExtension</see>,
    /// the context composition (<see cref="TinyMVC.Boot.Contexts.ControllersContext">ControllersContext</see>,
    /// <see cref="TinyMVC.Boot.Contexts.ModelsContext">ModelsContext</see>,
    /// <see cref="TinyMVC.Boot.Contexts.ParametersContext">ParametersContext</see>, views and modules)
    /// and the host platform hooks.
    /// </summary>
    public interface IContext : IUnloadLink {
        /// <summary> Unique context identifier. Unity: the name of the context GameObject. </summary>
        public string key { get; }
        
        /// <summary> Cancellation token of the context lifetime. </summary>
        public CancellationToken cancellation { get; }
        
        /// <summary> Per-controller unload pools. </summary>
        internal Dictionary<IController, UnloadPool> unloads { get; }
        
        /// <summary> Controllers composition of the context. Filled during <see cref="Create"/>. </summary>
        internal ControllersContext controllers { get; set; }
        
        /// <summary> Models composition of the context. Filled during <see cref="Create"/>. </summary>
        internal ModelsContext models { get; set; }
        
        /// <summary> Parameters composition of the context. Filled during <see cref="Create"/>. </summary>
        internal ParametersContext parameters { get; set; }
        
        /// <summary> Views composition of the context. </summary>
        internal IViewsContext views { get; }
        
        /// <summary> Modules attached to the context. </summary>
        internal IContextModule[] modules { get; }
        
        /// <summary> Scene identifier provided during registration. Unity: scene build index. </summary>
        internal int sceneId { get; set; }
        
        /// <summary> Pool unloaded with the current context. </summary>
        internal UnloadPool unloadPool { get; set; }
        
        /// <summary> Cancellation source of the context lifetime. </summary>
        internal CancellationTokenSource cancellationSource { get; set; }
        
        /// <summary> Set by the host when the context is fully initialized and all platform loop subscriptions are registered. </summary>
        internal bool isInitializationComplete { get; set; }
        
        /// <summary> Creates the user-defined controllers composition. </summary>
        internal ControllersContext CreateControllers();
        
        /// <summary> Creates the user-defined models composition. </summary>
        internal ModelsContext CreateModels();
        
        /// <summary> Creates the user-defined parameters composition. </summary>
        internal ParametersContext CreateParameters();
        
        /// <summary> Yields to the host frame loop. Returns unscaled elapsed time in seconds. </summary>
        internal Task<float> WaitFrame(CancellationToken cancellation);
        
        /// <summary> Stops the host pumping (Unity: StopAllCoroutines and editor subscriptions). </summary>
        internal void StopPumping();
        
        /// <summary> Marks the host as persistent across scenes. Called only for <see cref="IGlobalContext"/>. </summary>
        internal void MarkPersistent();
        
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