// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyMVC.ReactiveFields;

namespace TinyMVC.Modules.Networks.Buffers {
    internal sealed class NetReaderBuffer {
        public readonly ushort group;
        public readonly ushort part;
        public readonly byte key;
        public readonly List<ActionListener<ushort, object>> listeners;
        
        public NetReaderBuffer(ushort group, ushort part, byte key, ActionListener<ushort, object> listener) {
            this.group = group;
            this.part = part;
            this.key = key;
            listeners = new List<ActionListener<ushort, object>>() { listener };
        }
        
        public bool IsCurrent(ushort groupValue, ushort partValue, byte keyValue) => group == groupValue && part == partValue && key == keyValue;
    }
}