// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMVC.Boot.Extensions;
using TinyMVC.Dependencies;
using TinyMVC.Views;

namespace TinyMVC.Boot.Contexts {
    /// <summary>
    /// Views composition of the context required by the common initialization pipeline (<see cref="ContextExtension"/>).<br/>
    /// Implemented by <see cref="ViewsContext"/> inside Unity; outside Unity by a custom UI windows context.
    /// </summary>
    internal interface IViewsContext {
        /// <summary> Main views of the context, filled after <see cref="CreateViews"/>. </summary>
        internal List<IView> mainViews { get; }
        
        /// <summary> Creates platform instances of the pre-configured assets. </summary>
        internal void Instantiate();
        
        /// <summary> Collects view-based dependencies for the resolve stage. </summary>
        internal void GetDependencies(List<IDependency> dependencies);
        
        /// <summary> Creates user views and combines them with instantiated assets. </summary>
        internal void CreateViews();
        
        internal Task InitAsync();
        
        internal Task BeginPlay();
        
        /// <summary> Appends loop-compatible views into the tick collections. </summary>
        internal void CheckAndAdd<T>(List<T> collection);
        
        /// <summary> Marks persistent views on the platform side (Unity: DontDestroyOnLoad). </summary>
        internal void ApplyDontDestroyOnLoad();
    }
}