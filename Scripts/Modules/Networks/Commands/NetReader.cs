using System.Collections.Generic;
using TinyMVC.ReactiveFields;

namespace TinyMVC.Modules.Networks.Commands {
    internal sealed class NetReader {
        public readonly ushort group;
        public readonly ushort part;
        public readonly byte key;
        public readonly List<ActionListener<object>> listeners;
        
        public NetReader(ushort group, ushort part, byte key, ActionListener<object> listener) {
            this.group = group;
            this.part = part;
            this.key = key;
            listeners = new List<ActionListener<object>>() { listener };
        }
        
        public bool IsCurrent(ushort groupValue, ushort partValue, byte keyValue) => group == groupValue && part == partValue && key == keyValue;
    }
}