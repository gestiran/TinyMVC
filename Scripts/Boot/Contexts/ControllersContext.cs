using System;
using System.Collections.Generic;
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

        /// <summary> Create initialization stage </summary>
        /// <remarks> First create <see cref="_mainControllers"/> and fill pool </remarks>
        internal void Create() => Create(_mainControllers);

        /// <summary> Init initialization stage </summary>
        /// <remarks> Check and run <see cref="TinyMVC.Loop.IInit"/> interface on <see cref="_mainControllers"/> </remarks>
        internal void Init(Action<IController> connectController, Action<IController> disconnectController) {
            for (int controllerId = 0; controllerId < _mainControllers.Count; controllerId++) {
                if (_mainControllers[controllerId] is Controller controller) {
                    controller.ConnectToContext(connectController, disconnectController);
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

        internal void DeInitSubController(IController subController, Action<ILoop> removeLoop) {
            if (subController is ILoop loop) {
                removeLoop(loop);
            }

            if (subController is IUnload unload) {
                unload.Unload();
            }

            _subControllers.Remove(subController);
        }

        /// <summary> Unload initialization stage </summary>
        /// <remarks> Check and run <see cref="TinyMVC.Loop.IUnload"/> interface on all controllers </remarks>
        internal void Unload() {
            _subControllers.TryUnload();
            _mainControllers.TryUnload();
        }

        /// <summary> Create controllers and connect initialization </summary>
        /// <param name="controllers"> Controllers pool, placed in order of call </param>
        protected abstract void Create(List<IController> controllers);
    }
}