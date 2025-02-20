using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyMVC.Dependencies;

namespace TinyMVC.Controllers {
    public static class Controller {
        private static readonly RootSystem _system;
        
        private sealed class RootSystem : IController { }
        
        static Controller() => _system = new RootSystem();
        
        [Obsolete("Can't connect nothing!")]
        public static void Connect() { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Connect<T>() where T : IController, new() {
            T controller = new T();
            _system.Connect(controller);
            return controller;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Connect<T>(params IDependency[] dependencies) where T : IController, IResolving, new() {
            T controller = new T();
            _system.Connect(controller, dependencies);
            return controller;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T>(T controller) where T : IController {
            _system.Connect(controller);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T>(T controller, params IDependency[] dependencies) where T : IController, IResolving {
            _system.Connect(controller, dependencies);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect(List<IController> controller) {
            _system.Connect(controller);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect(List<IController> controllers, params IDependency[] dependencies) {
            _system.Connect(controllers, dependencies);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect(IController[] controller) {
            _system.Connect(controller);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect(IController[] controllers, params IDependency[] dependencies) {
            _system.Connect(controllers, dependencies);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1, T2>(T1 system, T2 controller) where T1 : IController where T2 : IController {
            system.Connect(controller);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1, T2>(T1 system, T2 controller, params IDependency[] dependencies) where T1 : IController where T2 : IController, IResolving {
            system.Connect(controller, dependencies);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(T1 system, List<IController> controllers) where T1 : IController {
            system.Connect(controllers);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(T1 system, List<IController> controllers, params IDependency[] dependencies) where T1 : IController {
            system.Connect(controllers, dependencies);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(T1 system, IController[] controllers) where T1 : IController {
            system.Connect(controllers);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Connect<T1>(T1 system, IController[] controllers, params IDependency[] dependencies) where T1 : IController {
            system.Connect(controllers, dependencies);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T>(T controller) where T : IController {
            _system.Disconnect(controller);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T>(T controller, int sceneId) where T : IController {
            _system.Disconnect(controller, sceneId);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1, T2>(T1 system, T2 controller) where T1 : IController where T2 : IController {
            system.Disconnect(controller);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Disconnect<T1, T2>(T1 system, T2 controller, int sceneId) where T1 : IController where T2 : IController {
            system.Disconnect(controller, sceneId);
        }
    }
}