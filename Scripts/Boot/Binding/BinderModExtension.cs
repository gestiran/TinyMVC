// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyReactive;

namespace TinyMVC.Boot.Binding {
    public static class BinderModExtension {
        public static T WithUnload<T>(this T binder, UnloadPool unload) where T : BinderMod {
            binder.ConnectUnload(unload);
            return binder;
        }
    }
}