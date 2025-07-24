using System;
using TinySerializer.Core.Misc;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Modules.Networks.ReactiveFields {
    public sealed class NetAction : IEquatable<NetAction> {
    #if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup, HideLabel, SuffixLabel("Type")]
    #endif
        public readonly ushort type;
        
    #if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup, HideLabel, SuffixLabel("Location")]
    #endif
        public readonly ushort locationId;
        
    #if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup, HideLabel, SuffixLabel("Section")]
    #endif
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