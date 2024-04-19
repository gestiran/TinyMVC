using System;
using JetBrains.Annotations;

namespace TinyMVC.Boot.Binding {
    internal static class BinderExtension {
        public static BinderLink AsLink<T>(this T binder) where T : Binder => new BinderLink(binder, binder.GetBindType());
        
        public static BinderLink AsLink<T>(this T binder, [NotNull] params Type[] types) where T : Binder => new BinderLink(binder, types);
    }
}