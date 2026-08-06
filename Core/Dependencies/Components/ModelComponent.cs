// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyMVC.Dependencies.Components {
    public abstract class ModelComponent : IEquatable<ModelComponent> {
        public int id { get; }
        
        protected ModelComponent() => id = GetId();
        
        private static int _globalId;
        
        public static int GetId() => _globalId++;
        
        public bool Equals(ModelComponent other) => other != null && other.id == id;
        
        public override int GetHashCode() => id;
    }
}