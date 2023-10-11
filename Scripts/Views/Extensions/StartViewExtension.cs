using System.Collections.Generic;

namespace TinyMVC.Views.Extensions {
    public static class StartViewExtension {
        public static void StartView<T>(this T[] views) where T : IStartView {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                views[viewId].StartView();
            }
        }
        
        public static void StartView<T>(this List<T> views) where T : IStartView {
            for (int viewId = 0; viewId < views.Count; viewId++) {
                views[viewId].StartView();
            }
        }
    }
}