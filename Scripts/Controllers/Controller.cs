using System;
using System.Collections.Generic;
using TinyMVC.Dependencies;

namespace TinyMVC.Controllers {
    public static class Controller {
        private static readonly RootSystem _system;
        
        private sealed class RootSystem : IController { }
        
        static Controller() => _system = new RootSystem();
        
        [Obsolete("Can't connect nothing!")]
        public static void Connect() { }
        
        public static T Connect<T>() where T : IController, new() {
            T controller = new T();
            _system.Connect(controller);
            return controller;
        }
        
        public static T Connect<T>(params IDependency[] dependencies) where T : IController, IResolving, new() {
            return _system.Connect<RootSystem, T>(dependencies);
        }
        
        public static void Connect<T>(T controller) where T : IController {
            _system.Connect(controller);
        }
        
        public static void Connect(List<IController> controller) {
            _system.Connect(controller);
        }
        
        public static void Connect(IController[] controller) {
            _system.Connect(controller);
        }
        
        public static void Connect<T1, T2>(T1 system, T2 controller) where T1 : IController where T2 : IController {
            system.Connect(controller);
        }
        
        public static void Connect<T1>(T1 system, List<IController> controllers) where T1 : IController {
            system.Connect(controllers);
        }
        
        public static void Connect<T1>(T1 system, IController[] controllers) where T1 : IController {
            system.Connect(controllers);
        }
        
        public static void Disconnect<T>(T controller) where T : IController {
            _system.Disconnect(controller);
        }
        
        public static void Disconnect<T>(T controller, string contextKey) where T : IController {
            _system.Disconnect(controller, contextKey);
        }
        
        public static void Disconnect<T1, T2>(T1 system, T2 controller) where T1 : IController where T2 : IController {
            system.Disconnect(controller);
        }
        
        public static void Disconnect<T1, T2>(T1 system, T2 controller, string contextKey) where T1 : IController where T2 : IController {
            system.Disconnect(controller, contextKey);
        }
    }
}