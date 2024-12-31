using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyMVC.Boot;
using TinyMVC.Dependencies;
using UnityEngine.SceneManagement;

namespace TinyMVC.Controllers {
    public static class ControllerExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 Connect<T1, T2>(this T1 system, T2 controller) where T1 : IController where T2 : IController {
            system.Connect(controller, SceneManager.GetActiveScene().buildIndex);
            return controller;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 Connect<T1, T2>(this T1 system, T2 controller, int sceneId) where T1 : IController where T2 : IController {
            SceneContext.GetContext(sceneId).Connect(system, controller, sceneId, ResolveUtility.Resolve);
            return controller;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 Connect<T1, T2>(this T1 system, T2 controller, params IDependency[] dependencies) where T1 : IController where T2 : IController, IResolving {
            Connect(system, controller, SceneManager.GetActiveScene().buildIndex, dependencies);
            return controller;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 Connect<T1, T2>(this T1 system, T2 controller, int sceneId, params IDependency[] dependencies)
            where T1 : IController where T2 : IController, IResolving {
            DependencyContainer container = new DependencyContainer(dependencies);
            SceneContext.GetContext(sceneId).Connect(system, controller, sceneId, resolving => ResolveUtility.Resolve(resolving, container));
            return controller;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T>(this T controller) where T : IController {
            Controller.Disconnect(controller);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1, T2>(this T1 system, T2 controller) where T1 : IController where T2 : IController {
            Disconnect(system, controller, SceneManager.GetActiveScene().buildIndex);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1>(this T1 system, List<IController> controllers) where T1 : IController {
            Disconnect(system, controllers, SceneManager.GetActiveScene().buildIndex);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1>(this T1 system, List<IController> controllers, int sceneId) where T1 : IController {
            for (int controllerId = 0; controllerId < controllers.Count; controllerId++) {
                Disconnect(system, controllers[controllerId], sceneId);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1, T2>(this T1 system, T2 controller, int sceneId) where T1 : IController where T2 : IController {
            SceneContext.GetContext(sceneId).Disconnect(system, controller, sceneId);
        }
    }
}