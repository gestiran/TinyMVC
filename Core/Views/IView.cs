// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Views {
    /// <summary>
    /// Platform-independent representation of the visual layer element.<br/>
    /// Inside Unity it is implemented by scene objects (<see cref="View"/>), outside Unity by UI windows.
    /// </summary>
    public interface IView {
        /// <summary> Root view that has connected the current view. </summary>
        public IView root { get; set; }

        /// <summary> Current connection state. </summary>
        public ConnectState connectState { get; set; }
    }
}