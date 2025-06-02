using System;

namespace TinyMVC.Modules.Networks.ReactiveFields {
    public sealed class NetSender<T> : IEquatable<NetSender<T>> {
        public readonly ushort group;
        public readonly ushort part;
        public readonly byte key;
        
        public NetSender(ushort group, ushort part, byte key) {
            this.group = group;
            this.part = part;
            this.key = key;
        }
        
        public void Send(T value) => NetService.Write(group, part, key, value);
        
        public override int GetHashCode() => HashCode.Combine(group, part, key);
        
        public bool Equals(NetSender<T> other) => other != null && other.group == group && other.part == part && other.key == key;
        
        public override bool Equals(object obj) => obj is NetSender<T> other && other.group == group && other.part == part && other.key == key;
    }
}