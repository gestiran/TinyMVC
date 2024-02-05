using System.Collections.Generic;
using TinyMVC.Controllers;
using TinyMVC.Extensions;

namespace TinyMVC.Boot {
    /// <summary> Contains controllers initialization </summary>
    public abstract class ControllersContext {
        private readonly List<IController> _controllers;

        protected ControllersContext() => _controllers = new List<IController>();
        
        internal void Create() => Create(_controllers);

        internal void Init() => _controllers.TryInit();

        internal void BeginPlay() => _controllers.TryBeginPlay();

        internal void CheckAndAdd<T>(List<T> list) {
            list.Capacity += list.Count;
            
            for (int controllerId = 0; controllerId < _controllers.Count; controllerId++) {
                if (_controllers[controllerId] is T controller) {
                    list.Add(controller);
                }
            }
        }

        internal void Unload() => _controllers.TryUnload();
        
        /// <summary> Create controllers and connect initialization </summary>
        /// <param name="controllers"> Controllers pool, placed in order of call </param>
        protected abstract void Create(List<IController> controllers);
    }
}