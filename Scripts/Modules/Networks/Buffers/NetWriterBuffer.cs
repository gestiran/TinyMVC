// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Modules.Networks.Buffers {
    internal sealed class NetWriterBuffer {
        public readonly ushort group;
        public readonly ushort part;
        public readonly byte key;
        public object value;
        
        public NetWriterBuffer(ushort group, ushort part, byte key, object value) {
            this.group = group;
            this.part = part;
            this.key = key;
            this.value = value;
        }
        
        public bool IsCurrent(ushort groupValue, ushort partValue, byte keyValue) => group == groupValue && part == partValue && key == keyValue;
    }
}