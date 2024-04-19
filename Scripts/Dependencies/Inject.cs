using System;

namespace TinyMVC.Dependencies {
    /// <summary> Inject field value in initialization </summary>
    /// <remarks> Injecting only in private\protected fields, required <see cref="IResolving"/> or <see cref="IApplyResolving"/> interface </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InjectAttribute : Attribute {
        internal bool isRequired { get; private set; }
        
        public InjectAttribute(bool isRequired = true) => this.isRequired = isRequired;
    }
}