// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Dependencies {
    /// <summary>
    /// Indicator of the presence of the <see cref="TinyMVC.Dependencies.IApplyResolving.ApplyResolving()">ApplyResolve</see> method.<br/>
    /// Part of the initialization system that runs after <see cref="TinyMVC.Loop.IInit">init</see>.
    /// </summary>
    public interface IApplyResolving {
        /// <summary>
        /// Initialization of start values and subscription to reactive fields.<br/>
        /// A place to get <see cref="TinyMVC.Dependencies.IDependency">dependencies</see> from the current <see cref="TinyMVC.Boot.SceneContext">SceneContext</see>.
        /// </summary>
        public void ApplyResolving();
    }
}