namespace TinyMVC.Modules.Networks.Commands {
    internal sealed class NetWriter {
        public readonly byte group;
        public readonly byte part;
        public readonly ushort key;
        public object value;
        
        public NetWriter(byte group, byte part, ushort key, object value) {
            this.group = group;
            this.part = part;
            this.key = key;
            this.value = value;
        }
        
        public bool IsCurrent(byte groupValue, byte partValue, ushort keyValue) => group == groupValue && part == partValue && key == keyValue;
    }
}