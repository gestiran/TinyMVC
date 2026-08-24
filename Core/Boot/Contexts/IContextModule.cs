// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyMVC.Controllers;
using TinyMVC.Dependencies;
using TinyMVC.Views;
using TinyReactive.Fields;

namespace TinyMVC.Boot.Contexts {
    /// <summary>
    /// Contract of a context module: additional source of controllers, models, binders, parameters and views.<br/>
    /// Inside Unity implemented by <see cref="TinyMVC.Boot.ContextComponent"/>; outside Unity by a plain class.
    /// </summary>
    internal interface IContextModule {
        /// <summary> Creates platform instances of the pre-configured assets. </summary>
        internal void Instantiate();
        
        /// <summary> Registers module controllers into the context systems list. </summary>
        internal void CreateControllersInternal(List<IController> systems, List<ActionListener> initSystemsLazy);
        
        /// <summary> Adds module views into the context main views list. </summary>
        internal void AddComponentsViews(List<IView> mainViews);
        
        /// <summary> Registers module parameters into the dependencies list. </summary>
        internal void CreateParametersInternal(List<IDependency> parameters);
        
        /// <summary> Runs binder creation against the models context. </summary>
        internal void CreateBindersInternal<T>(T context) where T : ModelsContext;
        
        /// <summary> Registers module models into the dependencies list. </summary>
        internal void CreateModelsInternal(List<IDependency> models);
    }
}