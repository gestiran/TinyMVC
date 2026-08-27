// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyReactive.Fields;

namespace TinyMVC.Boot.Contexts {
    internal interface IContextComponent {
        /// <summary> Creates platform instances of the pre-configured assets. </summary>
        internal void Instantiate();
        
        /// <summary> Registers module controllers into the context systems list. </summary>
        internal void CreateControllers(List<IController> systems, List<ActionListener> initSystemsLazy);
        
        /// <summary> Adds module views into the context main views list. </summary>
        internal void AddComponentsViews(IViewsContext context);
        
        /// <summary> Registers module parameters into the dependencies list. </summary>
        internal void CreateParameters(List<IDependency> parameters);
        
        /// <summary> Runs binder creation against the models context. </summary>
        internal void CreateBinders<T>(T context) where T : ModelsContext;
        
        /// <summary> Registers module models into the dependencies list. </summary>
        internal void CreateModels(List<IDependency> models);
    }
}