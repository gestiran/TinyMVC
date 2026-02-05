using System;

namespace TinyMVC.Boot.Binding {
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class RegisterBinderAttribute : Attribute {
        public readonly Type binderType;
        public readonly int priority;
        
        public RegisterBinderAttribute(Type binderType, int priority) : this(binderType) {
            this.priority = priority;
        }
        
        public RegisterBinderAttribute(Type binderType) {
            this.binderType = binderType;
        }
    }
}