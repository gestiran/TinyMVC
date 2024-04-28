using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Empty;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Loop.Extensions;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TinyMVC.Debugging.Exceptions;
#endif

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
        internal async Task InitAsync(int sceneId) {
            for (int controllerId = 0; controllerId < _mainControllers.Count; controllerId++) {
                if (_mainControllers[controllerId] is Controller controller) {
                    controller.sceneId = sceneId;
                }
            }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            try {
            #endif

                await _mainControllers.TryInitAsync();

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            } catch (InitException exception) {
                if (exception.other is IController controller) {
                    throw new ControllersException(controller, exception);
                }

                throw;
            } catch (InitAsyncException exception) {
                if (exception.other is IController controller) {
                    throw new ControllersException(controller, exception);
                }

                throw;
            } 
        #endif
        }

        /// <summary> Begin play initialization stage </summary>
        /// <remarks> Check and run <see cref="TinyMVC.Loop.IBeginPlay"/> interface on <see cref="_mainControllers"/> </remarks>
        internal async Task BeginPlay() {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            try {
            #endif

                await _mainControllers.TryBeginPlayAsync();

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            } catch (BeginPlayException exception) {
                if (exception.other is IController controller) {
                    throw new ControllersException(controller, exception);
                }

                throw;
            } catch (BeginPlayAsyncException exception) {
                if (exception.other is IController controller) {
                    throw new ControllersException(controller, exception);
                }

                throw;
            }
        #endif
        }

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

        internal void Connect(IController subController, int sceneId, Action<IResolving> resolve) {
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
                ProjectContext.current.ConnectLoop(sceneId, loop);
            }

            _subControllers.Add(subController);
        }
        
        internal void Disconnect(IController subController, int sceneId) {
            if (subController is ILoop loop) {
                ProjectContext.current.DisconnectLoop(sceneId, loop);
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

        protected void Add<T>() where T : IController, new() => _mainControllers.Add(new T());

        protected void Add<T>(T controller) where T : IController => _mainControllers.Add(controller);

        /// <summary> Create controllers and connect initialization </summary>
        protected abstract void Create();
    }
}