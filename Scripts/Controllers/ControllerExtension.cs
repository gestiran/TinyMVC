// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Views;

namespace TinyMVC.Controllers {
    public static class ControllerExtension {
        public static T2 Connect<T1, T2>(this T1 system, T2 controller) where T1 : IController where T2 : IController {
            string contextKey = ProjectContext.activeContext.key;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                context.Connect(system, controller, ResolveUtility.Resolve);
            }
            
            return controller;
        }
        
        public static T2 Connect<T1, T2>(this T1 system, T2 controller, string contextKey) where T1 : IController where T2 : IController {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                context.Connect(system, controller, ResolveUtility.Resolve);
            }
            
            return controller;
        }
        
        public static void Connect<T1>(this T1 system, List<IController> controllers) where T1 : IController {
            string contextKey = ProjectContext.activeContext.key;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                for (int controllerId = 0; controllerId < controllers.Count; controllerId++) {
                    context.Connect(system, controllers[controllerId], ResolveUtility.Resolve);
                }
            }
        }
        
        public static void Connect<T1>(this T1 system, List<IController> controllers, string contextKey) where T1 : IController {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                for (int controllerId = 0; controllerId < controllers.Count; controllerId++) {
                    context.Connect(system, controllers[controllerId], ResolveUtility.Resolve);
                }
            }
        }
        
        public static void Connect<T1>(this T1 system, IController[] controllers) where T1 : IController {
            string contextKey = ProjectContext.activeContext.key;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                    context.Connect(system, controllers[controllerId], ResolveUtility.Resolve);
                }
            }
        }
        
        public static void Connect<T1>(this T1 system, IController[] controllers, string contextKey) where T1 : IController {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                    context.Connect(system, controllers[controllerId], ResolveUtility.Resolve);
                }
            }
        }
        
        public static T2 Connect<T1, T2>(this T1 system, params IDependency[] dependencies) where T1 : IController where T2 : IController, IResolving, new() {
            string contextKey = ProjectContext.activeContext.key;
            T2 controller = default;
                
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                DependencyContainer container = new DependencyContainer(dependencies);
                ProjectContext.data.tempContainer = container;
                controller = new T2();
                
                context.Connect(system, controller, resolving => ResolveUtility.Resolve(resolving, container));
            }
            
            return controller;
        }
        
        public static T2 Connect<T1, T2>(this T1 system, string contextKey, params IDependency[] dependencies)
            where T1 : IController where T2 : IController, IResolving, new() {
            T2 controller = default;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                DependencyContainer container = new DependencyContainer(dependencies);
                ProjectContext.data.tempContainer = container;
                controller = new T2();
                
                context.Connect(system, controller, resolving => ResolveUtility.Resolve(resolving, container));
            }
            
            return controller;
        }
        
        public static void Disconnect<T>(this T controller) where T : IController {
            Controller.Disconnect(controller);
        }
        
        public static void Disconnect<T1, T2>(this T1 system, T2 controller) where T1 : IController where T2 : IController {
            string contextKey = ProjectContext.activeContext.key;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                context.Disconnect(system, controller);
            }
        }
        
        public static void Disconnect<T1>(this T1 system, List<IController> controllers) where T1 : IController {
            string contextKey = ProjectContext.activeContext.key;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                for (int controllerId = 0; controllerId < controllers.Count; controllerId++) {
                    context.Disconnect(system, controllers[controllerId]);
                }
            }
        }
        
        public static void Disconnect<T1>(this T1 system, List<IController> controllers, string contextKey) where T1 : IController {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                for (int controllerId = 0; controllerId < controllers.Count; controllerId++) {
                    context.Disconnect(system, controllers[controllerId]);
                }
            }
        }
        
        public static void Disconnect<T1>(this T1 system, IController[] controllers) where T1 : IController {
            string contextKey = ProjectContext.activeContext.key;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                    context.Disconnect(system, controllers[controllerId]);
                }
            }
        }
        
        public static void Disconnect<T1>(this T1 system, IController[] controllers, string contextKey) where T1 : IController {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                for (int controllerId = 0; controllerId < controllers.Length; controllerId++) {
                    context.Disconnect(system, controllers[controllerId]);
                }
            }
        }
        
        public static void Disconnect<T1, T2>(this T1 system, T2 controller, string contextKey) where T1 : IController where T2 : IController {
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                context.Disconnect(system, controller);
            }
        }
        
        public static void UpdateConnections(this IController system) {
            string contextKey = ProjectContext.activeContext.key;
            string systemName = system.GetType().Name;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                foreach (IController controller in context.controllers.ForEach(systemName)) {
                    if (controller is IUpdateConnection update) {
                        update.UpdateConnection();
                    }
                }
            }
        }
        
        public static void UpdateConnections<T>(this IController system) where T : IController {
            string contextKey = ProjectContext.activeContext.key;
            string systemName = system.GetType().Name;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                foreach (T controller in context.controllers.ForEach<T>(systemName)) {
                    if (controller is IUpdateConnection update) {
                        update.UpdateConnection();
                    }
                }
            }
        }
        
        public static void DisconnectAll(this IController system) {
            string contextKey = ProjectContext.activeContext.key;
            string systemName = system.GetType().Name;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                foreach (IController controller in context.controllers.ForEach(systemName)) {
                    context.Disconnect(system, controller);
                }
            }
        }
        
        public static void DisconnectAll(this IController system, string contextKey) {
            string systemName = system.GetType().Name;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                foreach (IController controller in context.controllers.ForEach(systemName)) {
                    context.Disconnect(system, controller);
                }
            }
        }
        
        public static void DisconnectAll<T>(this IController system) where T : IController {
            string contextKey = ProjectContext.activeContext.key;
            string systemName = system.GetType().Name;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                foreach (T controller in context.controllers.ForEach<T>(systemName)) {
                    context.Disconnect(system, controller);
                }
            }
        }
        
        public static void DisconnectAll<T>(this IController system, string contextKey) where T : IController {
            string systemName = system.GetType().Name;
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                foreach (T controller in context.controllers.ForEach<T>(systemName)) {
                    context.Disconnect(system, controller);
                }
            }
        }
    }
}