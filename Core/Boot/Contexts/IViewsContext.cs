// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Extensions;
using TinyMVC.Dependencies;
using TinyMVC.Views;

namespace TinyMVC.Boot.Contexts {
    /// <summary> Views composition of the context required by the common initialization pipeline (<see cref="ContextExtension"/>). </summary>
    internal interface IViewsContext {
        /// <summary> Creates platform instances of the pre-configured assets. </summary>
        internal void Instantiate();
        
        /// <summary> Collects view-based dependencies for the resolve stage. </summary>
        internal void GetDependencies(List<IDependency> dependencies);
        
        /// <summary> Creates user views and combines them with instantiated assets. </summary>
        internal void CreateViews();
        
        internal Task InitAsync();
        
        internal Task BeginPlay();
        
        internal void Unload();
        
        internal void AddView(IView view);
        
        internal void TryApplyResolving();
    }
}