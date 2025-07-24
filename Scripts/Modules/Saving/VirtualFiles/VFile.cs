// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyMVC.Modules.Saving.VirtualFiles {
    [Serializable]
    internal sealed class VFile : IEquatable<VFile> {
        public byte[] data;
        public readonly string name;
        
        public VFile(string name, byte[] data) : this(name) => this.data = data;
        
        private VFile(string name) => this.name = name;
        
        public bool Equals(VFile other) => other != null && other.name.Equals(name);
        
        public VFile GetClone() {
            VFile clone = new VFile(name);
            
            clone.data = new byte[data.Length];
            Array.Copy(data, clone.data, data.Length);
            
            return clone;
        }
    }
}