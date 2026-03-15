// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Boot.Contexts {
    /// <summary>
    /// Global <see cref="TinyMVC.Boot.SceneContext{T}">scene context</see> marker.<br/>
    /// All context components will remain public accessible after the scene is unloaded.<br/>
    /// </summary>
    public interface IGlobalContext { }
}