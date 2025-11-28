// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Modules.Saving.Reactive {
    public delegate bool SaveConfig<T>(T value, out T result);
}