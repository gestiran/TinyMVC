using System.Collections.Generic;
using TinyMVC.ReactiveFields;

namespace TinyMVC.Modules.Networks.Commands {
    internal sealed class NetReader {
        public readonly byte group;
        public readonly byte part;
        public readonly ushort key;
        public readonly List<ActionListener<object>> listeners;
        
        public NetReader(byte group, byte part, ushort key, ActionListener<object> listener) {
            this.group = group;
            this.part = part;
            this.key = key;
            listeners = new List<ActionListener<object>>() { listener };
        }
        
        public bool IsCurrent(byte groupValue, byte partValue, ushort keyValue) => group == groupValue && part == partValue && key == keyValue;
    }
}