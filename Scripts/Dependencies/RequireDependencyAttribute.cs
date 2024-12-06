using System;
using JetBrains.Annotations;

namespace TinyMVC.Dependencies {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RequireDependencyAttribute : Attribute {
        internal readonly Type[] types;
        
        [Obsolete("Not have any types!", true)]
        public RequireDependencyAttribute() => types = Type.EmptyTypes;
        
        public RequireDependencyAttribute([NotNull] params Type[] types) => this.types = types;
    }
}