using System;

namespace TinyMVC.Dependencies {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RemoveGroupAttribute : Attribute {
        internal string[] groups { get; private set; }
        
        [Obsolete("Can't remove nothing!", true)]
        public RemoveGroupAttribute() {
            // Do nothing!
        }
        
        public RemoveGroupAttribute(params string[] groups) => this.groups = groups;
    }
}