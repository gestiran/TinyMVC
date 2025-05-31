using System.Collections.Generic;
using TinyMVC.Modules.Networks.Extensions;
using TinyMVC.ReactiveFields;

namespace TinyMVC.Modules.Networks.Commands {
    internal sealed class NetReader {
        public readonly ushort key;
        public readonly byte[] group;
        public readonly List<ActionListener<object>> listeners;
        
        public NetReader(ActionListener<object> listener, ushort key, byte[] group) {
            this.key = key;
            this.group = group;
            listeners = new List<ActionListener<object>>() { listener };
        }
        
        public bool IsCurrent(ushort keyValue, byte[] groupValue) => key == keyValue && group.EqualsValues(groupValue);
    }
}