// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Boot.Contexts;
using TinyMVC.Boot;
using TinyReactive;

namespace TinyMVC.Loop.Extensions {
    internal static class LoopExtension {
        internal static void CheckAndAdd<T>(this ControllersContext context, List<T> collection) where T : ILoop {
            for (int systemId = 0; systemId < context.systems.Count; systemId++) {
                if (context.systems[systemId] is T controller) {
                    collection.Add(controller);
                }
            }
        }
        
        internal static void Connect<T1, T2>(this ControllersContext context, T2 system, T1 controller, Action<ILoop> connectLoop)
            where T1 : IController where T2 : IController {
            if (controller is IInit init) {
                init.Init();
            }
            
            if (controller is IApplyResolving applyResolving) {
                applyResolving.ApplyResolving();
            }
            
            if (controller is IBeginPlay beginPlay) {
                beginPlay.BeginPlay();
            }
            
            if (controller is ILoop loop) {
                connectLoop(loop);
            }
            
            string systemName = system.GetType().Name;
            
            if (context.controllers.TryGetValue(systemName, out List<IController> controllers)) {
                controllers.Add(controller);
            } else {
                context.controllers.Add(systemName, new List<IController>() { controller });
            }
        }
        
        internal static void Disconnect<T1, T2>(this ControllersContext context, T2 system, T1 controller, Action<ILoop> disconnectLoop)
            where T1 : IController where T2 : IController {
            if (controller is ILoop loop) {
                disconnectLoop(loop);
            }
            
            if (controller is IUnload unload) {
                unload.Unload();
            }
            
            if (ProjectContext.scene.unloads.Remove(controller, out UnloadPool globalUnload)) {
                globalUnload.Unload();
            }
            
            if (context.controllers.TryGetValue(system.GetType().Name, out List<IController> controllers)) {
                if (context.controllers.TryGetValue(controller.GetType().Name, out List<IController> subControllers)) {
                    for (int controllerId = subControllers.Count - 1; controllerId >= 0; controllerId--) {
                        context.DisconnectNR(system, subControllers[controllerId], disconnectLoop);
                    }
                }
                
                controllers.Remove(controller);
            }
        }
        
        private static void DisconnectNR<T1, T2>(this ControllersContext context, T2 system, T1 controller, Action<ILoop> disconnectLoop)
            where T1 : IController where T2 : IController {
            context.Disconnect(system, controller, disconnectLoop);
        }
    }
}