using System;
using JetBrains.Annotations;

namespace TinyMVC.Dependencies.Extensions {
    public static class DependencyExtension {
        public static Dependency AsDependency<T>(this T dependency, [NotNull] params Type[] types) where T : IDependency => new Dependency(dependency, types);
    }
}