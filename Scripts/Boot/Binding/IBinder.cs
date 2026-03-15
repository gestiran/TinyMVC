// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Dependencies;

namespace TinyMVC.Boot.Binding {
    /// <summary> The internal interface for initializing and loading models. </summary>
    internal interface IBinder {
        /// <summary> Creates and initializes the model. </summary>
        /// <returns> The model is ready for work. </returns>
        public IDependency GetDependency();
        
        /// <summary> Self-reference. </summary>
        public Binder current { get; }
    }
}