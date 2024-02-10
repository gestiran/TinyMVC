using System;
using System.Collections.Generic;
using TinyMVC.Boot.Empty;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Extensions;
using TinyMVC.Loop;

namespace TinyMVC.Boot.Contexts {
    /// <summary> Contains controllers initialization </summary>
    public abstract class ControllersContext {
        /// <summary> First created controllers pool </summary>
        /// <remarks> Created on <see cref="Create()"/> stage </remarks>
        private readonly List<IController> _mainControllers;

        /// <summary> Sub created controllers pool </summary>
        /// <remarks> Can be created using <see cref="Controller"/> controllers </remarks>
        private readonly List<IController> _subControllers;

        protected ControllersContext() {
            _mainControllers = new List<IController>();
            _subControllers = new List<IController>();
        }
        
        public static ControllersEmptyContext Empty() => new ControllersEmptyContext();

        /// <summary> Create initialization stage </summary>
        /// <remarks> First create <see cref="_mainControllers"/> and fill pool </remarks>
        internal void CreateControllers() => Create();

        /// <summary> Init initialization stage </summary>
        /// <remarks> Check and run <see cref="TinyMVC.Loop.IInit"/> interface on <see cref="_mainControllers"/> </remarks>
        internal void Init(Controller.Connector connector) {
            for (int controllerId = 0; controllerId < _mainControllers.Count; controllerId++) {
                if (_mainControllers[controllerId] is Controller controller) {
                    controller.ApplyConnector(connector);
                }
            }

            _mainControllers.TryInit();
        }

        /// <summary> Begin play initialization stage </summary>
        /// <remarks> Check and run <see cref="TinyMVC.Loop.IBeginPlay"/> interface on <see cref="_mainControllers"/> </remarks>
        internal void BeginPlay() => _mainControllers.TryBeginPlay();

        /// <summary> Add <see cref="_mainControllers"/> to list, if it contains a dependency of selected type </summary>
        /// <param name="list"> Reference list </param>
        /// <typeparam name="T"> Dependency type </typeparam>
        internal void CheckAndAdd<T>(List<T> list) {
            list.Capacity += list.Count;

            for (int controllerId = 0; controllerId < _mainControllers.Count; controllerId++) {
                if (_mainControllers[controllerId] is T controller) {
                    list.Add(controller);
                }
            }
        }

        internal void InitSubController(IController subController, Action<IResolving> resolve, Action<ILoop> addLoop) {
            if (subController is IInit init) {
                init.Init();
            }

            if (subController is IResolving resolving) {
                resolve(resolving);
            }

            if (subController is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }

            if (subController is ILoop loop) {
                addLoop(loop);
            }

            _subControllers.Add(subController);
        }
        
        internal void InitSubController(IController[] subControllers, Action<List<IResolving>> resolve, Action<ILoop> addLoop) {
            for (int controllerId = 0; controllerId < subControllers.Length; controllerId++) {
                if (subControllers[controllerId] is IInit init) {
                    init.Init();
                }
            }
            
            List<IResolving> all = new List<IResolving>();
            
            for (int controllerId = 0; controllerId < subControllers.Length; controllerId++) {
                if (subControllers[controllerId] is IResolving resolving) {
                    all.Add(resolving);
                }
            }

            if (all.Count > 0) {
                resolve(all);
            }
            
            for (int controllerId = 0; controllerId < subControllers.Length; controllerId++) {
                if (subControllers[controllerId] is IBeginPlay beginPlay) {
                    beginPlay.BeginPlay();
                }
            }
            
            for (int controllerId = 0; controllerId < subControllers.Length; controllerId++) {
                if (subControllers[controllerId] is ILoop loop) {
                    addLoop(loop);
                }
            }

            _subControllers.AddRange(subControllers);
        }

        internal void DeInitSubController(IController subController, Action<ILoop> removeLoop) {
            if (subController is ILoop loop) {
                removeLoop(loop);
            }

            if (subController is IUnload unload) {
                unload.Unload();
            }

            _subControllers.Remove(subController);
        }
        
        internal void DeInitSubController(IController[] subControllers, Action<ILoop> removeLoop) {
            for (int controllerId = 0; controllerId < subControllers.Length; controllerId++) {
                if (subControllers[controllerId] is ILoop loop) {
                    removeLoop(loop);
                }
            }
            
            for (int controllerId = 0; controllerId < subControllers.Length; controllerId++) {
                if (subControllers[controllerId] is IUnload unload) {
                    unload.Unload();
                }
            }

            for (int controllerId = 0; controllerId < subControllers.Length; controllerId++) {
                _subControllers.Remove(subControllers[controllerId]);
            }
        }

        /// <summary> Unload initialization stage </summary>
        /// <remarks> Check and run <see cref="TinyMVC.Loop.IUnload"/> interface on all controllers </remarks>
        internal void Unload() {
            _subControllers.TryUnload();
            _mainControllers.TryUnload();
        }

        protected void Add<T>() where T : IController, new() => _mainControllers.Add(new T());
        
        protected void Add<T>(T controller) where T : IController => _mainControllers.Add(controller);

        /// <summary> Create controllers and connect initialization </summary>
        protected abstract void Create();
    }
}