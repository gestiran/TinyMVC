using JetBrains.Annotations;
using TinyMVC.Boot;
using TinyMVC.Dependencies;

namespace TinyMVC.Controllers {
    /// <summary> Project logic container </summary>
    /// <remarks>
    /// First created on any <see cref="TinyMVC.Boot.SceneContext"/>.<see cref="TinyMVC.Boot.SceneContext{T}.CreateControllers()"/>,
    /// сan create additional controllers and add them to the initialization
    /// </remarks>
    public abstract class Controller : IController {
        public ConnectState connectState { get; private set; }
        
        internal int sceneId;
        
        public enum ConnectState : byte { Disconnected, Connected }
        
        protected T ConnectController<T>() where T : class, IController, new() {
            T controller = new T();
            TryApply(controller, ConnectState.Connected);
            SceneContext.GetContext(sceneId).Connect(controller, sceneId, ProjectContext.data.Resolve);
            
            return controller;
        }
        
        protected T ConnectController<T>([NotNull] T controller) where T : class, IController {
            TryApply(controller, ConnectState.Connected);
            SceneContext.GetContext(sceneId).Connect(controller, sceneId, ProjectContext.data.Resolve);
            
            return controller;
        }
        
        protected void ConnectController([NotNull] params IController[] controllers) {
            for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                TryApply(controllers[controllerId], ConnectState.Connected);
                SceneContext.GetContext(sceneId).Connect(controllers[controllerId], sceneId, ProjectContext.data.Resolve);
            }
        }
        
        protected T ConnectController<T>([NotNull] params IDependency[] dependencies) where T : class, IController, IResolving, new() {
            return ConnectController(new T(), dependencies);
        }
        
        protected T ConnectController<T>([NotNull] T controller, [NotNull] params IDependency[] dependencies) where T : class, IController, IResolving {
            return ConnectController(controller, new DependencyContainer(dependencies));
        }
        
        protected T ConnectController<T>([NotNull] T controller, [NotNull] IDependency dependency) where T : class, IController, IResolving {
            return ConnectController(controller, new DependencyContainer(dependency));
        }
        
        protected T ConnectController<T>([NotNull] T controller, [NotNull] DependencyContainer container) where T : class, IController, IResolving {
            TryApply(controller, ConnectState.Connected);
            SceneContext.GetContext(sceneId).Connect(controller, sceneId, resolving => ProjectContext.data.Resolve(container, resolving));
            
            return controller;
        }
        
        protected void DisconnectController() => DisconnectController(this);
        
        protected void DisconnectController<T>([NotNull] T controller) where T : class, IController {
            SceneContext.GetContext(sceneId).Disconnect(controller, sceneId);
            TryApply(controller, ConnectState.Disconnected);
        }
        
        protected void DisconnectController([NotNull] params IController[] controllers) {
            for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                SceneContext.GetContext(sceneId).Disconnect(controllers[controllerId], sceneId);
                TryApply(controllers[controllerId], ConnectState.Disconnected);
            }
        }
        
        public T ReconnectController<T>([NotNull] T controllers, [NotNull] IDependency dependency) where T : Controller, IResolving {
            if (controllers.connectState == ConnectState.Connected) {
                DisconnectController(controllers);
            }
            
            return ConnectController(controllers, dependency);
        }
        
        public T ReconnectController<T>([NotNull] T controller, [NotNull] params IDependency[] dependencies) where T : Controller, IResolving {
            if (controller.connectState == ConnectState.Connected) {
                DisconnectController(controller);
            }
            
            return ConnectController(controller, dependencies);
        }
        
        private void TryApply<T>(T controller, ConnectState state) where T : class, IController {
            if (controller is Controller link) {
                link.sceneId = sceneId;
                link.connectState = state;
            }
        }
    }
}