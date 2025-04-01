using System;
using Sirenix.OdinInspector;

namespace TinyMVC.Boot.Helpers {
    internal abstract class ContextLink<T> : IEquatable<ContextLink<T>> {
        [HideInEditorMode, HideInPlayMode]
        public readonly string contextKey;
        
        [ShowInInspector, Title("@contextKey"), InlineProperty, HideLabel, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox]
        public readonly T context;
        
        protected ContextLink(string contextKey, T context) {
            this.contextKey = contextKey;
            this.context = context;
        }
        
        public bool Equals(ContextLink<T> other) => other != null && contextKey.Equals(other.contextKey);
        
        public override bool Equals(object obj) => obj is ContextLink<T> other && contextKey.Equals(other.contextKey);
        
        public override int GetHashCode() => contextKey.GetHashCode();
    }
}