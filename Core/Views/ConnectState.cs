// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Views {
    /// <summary> Connection state of the <see cref="IView"/> inside the current <see cref="TinyMVC.Boot.SceneContext">SceneContext</see>. </summary>
    public enum ConnectState : byte {
        Disconnected,
        Connected
    }
}