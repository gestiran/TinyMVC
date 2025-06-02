namespace TinyMVC.Modules.Networks.Commands {
    internal sealed class NetWriter {
        public readonly ushort group;
        public readonly ushort part;
        public readonly byte key;
        public object value;
        
        public NetWriter(ushort group, ushort part, byte key, object value) {
            this.group = group;
            this.part = part;
            this.key = key;
            this.value = value;
        }
        
        public bool IsCurrent(ushort groupValue, ushort partValue, byte keyValue) => group == groupValue && part == partValue && key == keyValue;
    }
}