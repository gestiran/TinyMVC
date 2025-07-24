// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Views.Extensions {
    public static class ViewExtension {
        public static View[] AsBaseView<T>(this T[] views) where T : View {
            View[] result = new View[views.Length];
            
            for (int i = 0; i < views.Length; i++) {
                result[i] = views[i];
            }
            
            return result;
        }
        
        public static T[] AsTargetView<T>(this View[] views) where T : View {
            T[] result = new T[views.Length];
            
            for (int i = 0; i < views.Length; i++) {
                result[i] = views[i] as T;
            }
            
            return result;
        }
    }
}