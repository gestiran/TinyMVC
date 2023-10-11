using System.Collections.Generic;

namespace TinyMVC.Views.Extensions {
    public static class InitViewExtension {
        public static void Init<T>(this T[] views) where T : IInitView {
            for (int viewId = 0; viewId < views.Length; viewId++) {
                views[viewId].Init();
            }
        }
        
        public static void Init<T>(this List<T> views) where T : IInitView {
            for (int viewId = 0; viewId < views.Count; viewId++) {
                views[viewId].Init();
            }
        }
    }
}