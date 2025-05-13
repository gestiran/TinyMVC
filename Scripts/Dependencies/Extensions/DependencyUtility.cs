using System;
using System.Reflection;

namespace TinyMVC.Dependencies.Extensions {
    public static class DependencyUtility {
        internal static Type[] GetRequirements(Type type) {
            RequireDependencyAttribute attribute = (RequireDependencyAttribute)type.GetCustomAttribute(typeof(RequireDependencyAttribute));
            
            if (attribute == null) {
                return Type.EmptyTypes;
            }
            
            return attribute.types;
        }
    }
}