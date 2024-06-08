using System;

namespace TinyMVC.Dependencies {
    public sealed class Dependency : IDependency {
        internal readonly IDependency link;
        internal readonly Type[] types;
        
        internal Dependency(IDependency link, params Type[] types) {
            this.link = link;
            this.types = types;
        }
    }
}