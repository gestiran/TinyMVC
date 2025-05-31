using System;
using TinyMVC.Modules.Networks.Extensions;

namespace TinyMVC.Modules.Networks.ReactiveFields {
    public sealed class NetSender<T> : IEquatable<NetSender<T>> {
        public readonly ushort key;
        public readonly byte[] group;
        
        [Obsolete("Can`t use without parameters!", true)]
        public NetSender(ushort key) {
            // Do Nothing
        }
        
        public NetSender(ushort key, params byte[] group) {
            this.key = key;
            this.group = group;
        }
        
        public void Send(T value) => NetService.Write(value, key, group);
        
        public override int GetHashCode() => HashCode.Combine(key, group);
        
        public bool Equals(NetSender<T> other) => other != null && other.key == key && other.group.EqualsValues(group);
        
        public override bool Equals(object obj) => obj is NetSender<T> other && other.key == key && other.group.EqualsValues(group);
    }
}