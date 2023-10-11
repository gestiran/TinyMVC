using System;
using System.Collections.Generic;
using TinyMVC.Controllers;
using TinyMVC.Utilities.Updating;
using TinyDI.Dependencies;
using TinyDI.Dependencies.Models;
using TinyDI.Dependencies.Parameters;

namespace TinyMVC.Boot {
    public abstract class BootControllers : IDisposable {
        protected List<IController> _controllers;

        public virtual void Create() {
            _controllers = new List<IController>();
        }

        public virtual void Dispose() {
            for (int controllerId = 0; controllerId < _controllers.Count; controllerId++) {
                UpdateLoopUtility.TryRemoveSystem(_controllers[controllerId]);
            }
            
            for (int controllerId = 0; controllerId < _controllers.Count; controllerId++) {
                if (_controllers[controllerId] is IDisposable controller) {
                    controller.Dispose();
                }
            }
        }

        public void GetParametersResolvers(List<IParametersResolving> resolving) => GetResolvers(resolving);

        public void GetModelsResolvers(List<IModelsResolving> resolving) => GetResolvers(resolving);

        public void Start() {
            for (int controllerId = 0; controllerId < _controllers.Count; controllerId++) {
                if (_controllers[controllerId] is IStartController controller) {
                    controller.Start();
                }
            }
        } 
        
        public void StartUpdateLoop() {
            for (int controllerId = 0; controllerId < _controllers.Count; controllerId++) {
                UpdateLoopUtility.TryAddSystem(_controllers[controllerId]);
            }
        }

        private void GetResolvers<T>(List<T> resolving) where T : IResolving {
            for (int controllerId = 0; controllerId < _controllers.Count; controllerId++) {
                if (_controllers[controllerId] is T item) {
                    resolving.Add(item);
                }
            }
        }
    }
}