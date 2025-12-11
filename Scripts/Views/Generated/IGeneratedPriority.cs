// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Views.Generated {
    /// <summary>
    /// Allows you to set the initialization priority for <see cref="TinyMVC.Views.View">View</see>.
    /// Also affects the priority of the <see cref="TinyMVC.Views.View.Reset">Reset</see> call.
    /// </summary>
    public interface IGeneratedPriority {
        /// <summary>
        /// The <see cref="TinyMVC.Views.View">View</see> with higher priority will be executed first.
        /// Default priority is 0.
        /// </summary>
        public int priority { get; }
    }
}