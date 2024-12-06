using System;

namespace TinyMVC.Dependencies {
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InjectAttribute : Attribute {
        internal bool isRequired { get; private set; }
        
        public InjectAttribute(bool isRequired = true) => this.isRequired = isRequired;
    }
}