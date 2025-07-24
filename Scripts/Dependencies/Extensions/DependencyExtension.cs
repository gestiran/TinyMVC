// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using JetBrains.Annotations;

namespace TinyMVC.Dependencies.Extensions {
    public static class DependencyExtension {
        [Obsolete("Can't use without parameters!", true)]
        public static Dependency AsDependency<T>(this T _) where T : IDependency => null;
        
        public static Dependency AsDependency<T>(this T dependency, [NotNull] params Type[] types) where T : IDependency => new Dependency(dependency, types);
        
        internal static Type[] AsRequirements(this IDependency[] dependencies) {
            Type[] result = new Type[dependencies.Length];
            
            for (int i = 0; i < dependencies.Length; i++) {
                result[i] = dependencies[i].GetType();
            }
            
            return result;
        }
    }
}