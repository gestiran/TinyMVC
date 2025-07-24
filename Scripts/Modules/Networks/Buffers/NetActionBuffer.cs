// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Modules.Networks.Buffers {
    internal sealed class NetActionBuffer {
        public readonly ushort type;
        public readonly ushort locationId;
        public readonly byte section;
        
        public byte[] data;
        
        public NetActionBuffer(ushort type, ushort location, byte section, byte[] data) {
            this.type = type;
            locationId = location;
            this.section = section;
            this.data = data;
        }
        
        public bool IsCurrent(ushort typeValue, ushort locationValue, byte sectionValue) => typeValue == type && locationValue == locationId && sectionValue == section;
    }
}