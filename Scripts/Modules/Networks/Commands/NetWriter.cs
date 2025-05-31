using TinyMVC.Modules.Networks.Extensions;

namespace TinyMVC.Modules.Networks.Commands {
    internal sealed class NetWriter {
        public object value;
        public readonly ushort key;
        public readonly byte[] group;
        
        public NetWriter(object value, ushort key, byte[] group) {
            this.value = value;
            this.key = key;
            this.group = group;
        }
        
        public bool IsCurrent(ushort keyValue, byte[] groupValue) => key == keyValue && group.EqualsValues(groupValue);
    }
}