// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyMVC.Dependencies {
    /// <summary> Dependency indicator used in resolve. </summary>
    public interface IDependency {
        /// <summary> Gets the <see cref="System.Type">type</see> of the current instance. </summary>
        /// <returns> The exact dependency type of the current instance. </returns>
        public Type GetType();
    }
}