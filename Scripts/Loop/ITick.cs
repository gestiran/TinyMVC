// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Loop {
    public interface ITick : ILoop {
        public void Tick();
    }
}