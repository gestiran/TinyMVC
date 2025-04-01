using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyMVC.Boot;
using TinyMVC.Dependencies;

namespace TinyMVC.Controllers {
    public static class ControllerExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 Connect<T1, T2>(this T1 system, T2 controller) where T1 : IController where T2 : IController {
            system.Connect(controller, ProjectContext.activeContext.key);
            return controller;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 Connect<T1, T2>(this T1 system, T2 controller, string contextKey) where T1 : IController where T2 : IController {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                context.Connect(system, controller, ResolveUtility.Resolve);
            }
            
            return controller;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(this T1 system, List<IController> controllers) where T1 : IController {
            string contextKey = ProjectContext.activeContext.key;
            
            for (int controllerId = 0; controllerId < controllers.Count; controllerId++) {
                system.Connect(controllers[controllerId], contextKey);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(this T1 system, List<IController> controllers, string contextKey) where T1 : IController {
            for (int controllerId = 0; controllerId < controllers.Count; controllerId++) {
                system.Connect(controllers[controllerId], contextKey);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(this T1 system, IController[] controllers) where T1 : IController {
            string contextKey = ProjectContext.activeContext.key;
            
            for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                system.Connect(controllers[controllerId], contextKey);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(this T1 system, IController[] controllers, string contextKey) where T1 : IController {
            for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                system.Connect(controllers[controllerId], contextKey);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 Connect<T1, T2>(this T1 system, T2 controller, params IDependency[] dependencies) where T1 : IController where T2 : IController, IResolving {
            Connect(system, controller, ProjectContext.activeContext.key, dependencies);
            return controller;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 Connect<T1, T2>(this T1 system, T2 controller, string contextKey, params IDependency[] dependencies)
            where T1 : IController where T2 : IController, IResolving {
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                DependencyContainer container = new DependencyContainer(dependencies);
                ProjectContext.data.tempContainer = container;
                
                context.Connect(system, controller, resolving => ResolveUtility.Resolve(resolving, container));
            }
            
            return controller;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(this T1 system, List<IController> controllers, params IDependency[] dependencies) where T1 : IController {
            string contextKey = ProjectContext.activeContext.key;
            DependencyContainer container = new DependencyContainer(dependencies);
            ProjectContext.data.tempContainer = container;
            
            for (int controllerId = 0; controllerId < controllers.Count; controllerId++) {
                if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                    context.Connect(system, controllers[controllerId], resolving => ResolveUtility.Resolve(resolving, container));
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(this T1 system, List<IController> controllers, string contextKey, params IDependency[] dependencies) where T1 : IController {
            DependencyContainer container = new DependencyContainer(dependencies);
            ProjectContext.data.tempContainer = container;
            
            for (int controllerId = 0; controllerId < controllers.Count; controllerId++) {
                if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                    context.Connect(system, controllers[controllerId], resolving => ResolveUtility.Resolve(resolving, container));
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(this T1 system, IController[] controllers, params IDependency[] dependencies) where T1 : IController {
            string contextKey = ProjectContext.activeContext.key;
            DependencyContainer container = new DependencyContainer(dependencies);
            ProjectContext.data.tempContainer = container;
            
            for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                    context.Connect(system, controllers[controllerId], resolving => ResolveUtility.Resolve(resolving, container));
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(this T1 system, IController[] controllers, string contextKey, params IDependency[] dependencies) where T1 : IController {
            DependencyContainer container = new DependencyContainer(dependencies);
            ProjectContext.data.tempContainer = container;
            
            for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                    context.Connect(system, controllers[controllerId], resolving => ResolveUtility.Resolve(resolving, container));
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T>(this T controller) where T : IController {
            Controller.Disconnect(controller);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1, T2>(this T1 system, T2 controller) where T1 : IController where T2 : IController {
            Disconnect(system, controller, ProjectContext.activeContext.key);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1>(this T1 system, List<IController> controllers) where T1 : IController {
            Disconnect(system, controllers, ProjectContext.activeContext.key);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1>(this T1 system, List<IController> controllers, string contextKey) where T1 : IController {
            for (int controllerId = 0; controllerId < controllers.Count; controllerId++) {
                Disconnect(system, controllers[controllerId], contextKey);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1>(this T1 system, IController[] controllers) where T1 : IController {
            Disconnect(system, controllers, ProjectContext.activeContext.key);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1>(this T1 system, IController[] controllers, string contextKey) where T1 : IController {
            for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                Disconnect(system, controllers[controllerId], contextKey);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1, T2>(this T1 system, T2 controller, string contextKey) where T1 : IController where T2 : IController {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                context.Disconnect(system, controller);
            }
        }
    }
}