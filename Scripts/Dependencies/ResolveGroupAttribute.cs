using System;

namespace TinyMVC.Dependencies {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ResolveGroupAttribute : Attribute {
        internal string group { get; private set; }
        
        public ResolveGroupAttribute(string group) => this.group = group;
    }
}