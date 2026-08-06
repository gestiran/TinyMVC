// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Loop {
    /// <summary>
    /// Indicator of the presence of the <see cref="TinyMVC.Loop.IInit.Init()">Init</see> method.<br/>
    /// Part of the initialization system that runs first after scene load complete, before <see cref="TinyMVC.Boot.Binding.Binder{T}">binding</see>.
    /// </summary>
    public interface IInit {
        /// <summary>
        /// Initialization of <see cref="TinyMVC.Views.View">View</see> parameters before <see cref="TinyMVC.Dependencies.IDependency">dependencies</see> are resolved.
        /// </summary>
        public void Init();
    }
}