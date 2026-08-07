// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyMVC.Boot.Helpers {
    internal abstract class ContextLink<T> : IEquatable<ContextLink<T>> {
        public readonly string contextKey;
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