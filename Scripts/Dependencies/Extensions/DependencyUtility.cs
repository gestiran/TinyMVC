using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TinyMVC.Dependencies.Extensions {
    public static class DependencyUtility {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Type[] GetRequirements(Type type) {
            RequireDependencyAttribute attribute = (RequireDependencyAttribute)type.GetCustomAttribute(typeof(RequireDependencyAttribute));
            
            if (attribute == null) {
                return Type.EmptyTypes;
            }
            
            return attribute.types;
        }
    }
}