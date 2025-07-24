// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Modules.ADS {
    internal sealed class AsyncCancellation {
        public bool isCancel { get; private set; }
        
        public void Cancel() => isCancel = true;
    }
}