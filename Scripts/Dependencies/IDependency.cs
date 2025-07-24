// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies {
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public interface IDependency {
        public Type GetType();
    }
}