// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Loop {
    /// <summary>
    /// Update marker for <see cref="TinyMVC.Views.View">View</see> and <see cref="TinyMVC.Controllers.IController">Controller</see>.<br/>
    /// - FixedUpdate as <see cref="TinyMVC.Loop.IFixedTick">FixedTick</see>.<br/>
    /// - Update as <see cref="TinyMVC.Loop.ITick">Tick</see>.<br/>
    /// - LateUpdate as <see cref="TinyMVC.Loop.ILateTick">LateTick</see>.<br/>
    /// </summary>
    public interface ILoop { }
}