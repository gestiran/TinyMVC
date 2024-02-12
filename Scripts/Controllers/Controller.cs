using System;
using JetBrains.Annotations;
using TinyMVC.Dependencies;
using TinyMVC.Loop;

namespace TinyMVC.Controllers {
    /// <summary> Project logic container </summary>
    /// <remarks>
    /// First created on any <see cref="TinyMVC.Boot.SceneContext"/>.<see cref="TinyMVC.Boot.SceneContext.CreateControllers()"/>,
    /// сan create additional controllers and add them to the initialization
    /// </remarks>
    public abstract class Controller : IController {
        /// <summary> Connect to initialization</summary>
        private Connector _connector;

        /// <summary> Contains links to connect to initialization </summary>
        internal sealed class Connector {
            internal Action<IController> connect;
            internal Action<IController> connectWithoutDependencies;
            internal Action<IController[]> connectArray;
            internal Action<IController> disconnect;
            internal Action<IController[]> disconnectArray;
        }
        
        internal void ApplyConnector(Connector connector) => _connector = connector;

        protected T ConnectController<T>() where T : class, IController, new() {
            T controller = new T();
            TryApplyConnector(controller);
            _connector.connect(controller);
            return controller;
        }
        
        protected T ConnectController<T>(UnloadPool pool) where T : class, IController, new() {
            T controller = new T();
            TryApplyConnector(controller);
            _connector.connect(controller);
            pool.Add(new UnloadAction(() => DisconnectController(controller)));
            return controller;
        }
        
        protected T ConnectController<T>([NotNull] T controller) where T : class, IController {
            TryApplyConnector(controller);
            _connector.connect(controller);
            return controller;
        }
        
        protected T ConnectController<T>([NotNull] T controller, UnloadPool pool) where T : class, IController {
            TryApplyConnector(controller);
            _connector.connect(controller);
            pool.Add(new UnloadAction(() => DisconnectController(controller)));
            return controller;
        }
        
        protected void ConnectController([NotNull] params IController[] controllers) {
            for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                TryApplyConnector(controllers[controllerId]);
            }
            
            _connector.connectArray(controllers);
        }
        
        protected void ConnectController(UnloadPool pool, [NotNull] params IController[] controllers) {
            for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                TryApplyConnector(controllers[controllerId]);
            }
            
            _connector.connectArray(controllers);

            foreach (IController controller in controllers) {
                pool.Add(new UnloadAction(() => DisconnectController(controller)));   
            }
        }
        
        protected T ConnectController<T>(UnloadPool pool, [NotNull] IDependency dependency) where T : class, IController, IResolving, new() {
            T controller = new T();
            TryApplyConnector(controller);
            _connector.connectWithoutDependencies(controller);
            ResolveUtility.Resolve(controller, new DependencyContainer(dependency));
            pool.Add(new UnloadAction(() => DisconnectController(controller)));
            return controller;
        }
        
        protected T ConnectController<T>(UnloadPool pool, [NotNull] params IDependency[] dependencies) where T : class, IController, IResolving, new() {
            T controller = new T();
            TryApplyConnector(controller);
            _connector.connectWithoutDependencies(controller);
            ResolveUtility.Resolve(controller, new DependencyContainer(dependencies));
            pool.Add(new UnloadAction(() => DisconnectController(controller)));
            return controller;
        }
        
        protected T ConnectController<T>([NotNull] T controller, [NotNull] IDependency dependency) where T : class, IController, IResolving {
            TryApplyConnector(controller);
            _connector.connectWithoutDependencies(controller);
            ResolveUtility.Resolve(controller, new DependencyContainer(dependency));
            return controller;
        }
        
        protected T ConnectController<T>([NotNull] T controller, UnloadPool pool, [NotNull] params IDependency[] dependencies) where T : class, IController, IResolving {
            TryApplyConnector(controller);
            _connector.connectWithoutDependencies(controller);
            ResolveUtility.Resolve(controller, new DependencyContainer(dependencies));
            pool.Add(new UnloadAction(() => DisconnectController(controller)));
            return controller;
        }

        protected void DisconnectController<T>([NotNull] T controller) where T : class, IController => _connector.disconnect(controller);
        
        protected void DisconnectController([NotNull] params IController[] controllers) => _connector.disconnectArray(controllers);

        private bool TryApplyConnector<T>(T controller) where T : class, IController {
            if (controller is not Controller root) {
                return false;
            }

            root.ApplyConnector(_connector);
            return true;
        }
    }
}