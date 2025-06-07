using System;
using Sirenix.OdinInspector;
using TinySerializer.Core.Misc;

namespace TinyMVC.Modules.Networks.ReactiveFields {
    public sealed class NetAction : IEquatable<NetAction> {
        [ShowInInspector, HorizontalGroup, HideLabel, SuffixLabel("Type")]
        public readonly ushort type;
        
        [ShowInInspector, HorizontalGroup, HideLabel, SuffixLabel("Location")]
        public readonly ushort locationId;
        
        [ShowInInspector, HorizontalGroup, HideLabel, SuffixLabel("Section")]
        public readonly byte section;
        
        public NetAction(ushort type, ushort location, byte section) {
            this.type = type;
            locationId = location;
            this.section = section;
        }
        
        public void Send<T>(T command) where T : unmanaged {
            NetSyncService.Action(type, locationId, section, SerializationUtility.SerializeValueWeak(command, DataFormat.Binary));
        }
        
        public bool Equals(NetAction other) => other != null && type == other.type && locationId == other.locationId && section == other.section;
        
        public override bool Equals(object obj) => obj is NetAction other && type == other.type && locationId == other.locationId && section == other.section;
        
        public override int GetHashCode() => HashCode.Combine(type, locationId, section);
    }
}