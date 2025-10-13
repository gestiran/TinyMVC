using System;
using TinyMVC.Boot;

namespace TinyMVC.Dependencies.Extensions {
    public static class ApplyResolveExtension {
        [Obsolete("Can't use without parameters!", true)]
        public static void ApplyResolving<T>(this T target) where T : IApplyResolving {
            // Do nothing
        }
        
        public static void ApplyResolving<T>(this T target, params IDependency[] dependencies) where T : IApplyResolving {
            DependencyContainer container = new DependencyContainer(dependencies);
            ProjectContext.data.tempContainer = container;
            target.ApplyResolving();
        }
    }
}