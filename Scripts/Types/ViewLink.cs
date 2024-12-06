using System;
using TinyMVC.Dependencies;
using TinyMVC.Views;

namespace TinyMVC.Types {
    public sealed class ViewLink : IEquatable<View> {
        internal readonly View link;
        
        private ViewLink(View link) => this.link = link;
        
        public static implicit operator ViewLink(View view) => new ViewLink(view);
        
        public IDependency AsDependency() => link as IDependency;
        
        public bool Equals(View other) => link.Equals(other);
        
        public new Type GetType() => link.GetType();
        
        public override int GetHashCode() => link.GetHashCode();
    }
}