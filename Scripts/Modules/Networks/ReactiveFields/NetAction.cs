using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TinyMVC.Modules.Networks.ReactiveFields {
    public sealed class NetAction : IEquatable<NetAction> {
        [ShowInInspector, HorizontalGroup, HideLabel, SuffixLabel("Type")]
        public readonly ushort type;
        
        [ShowInInspector, HorizontalGroup, HideLabel, SuffixLabel("Location")]
        public readonly ushort locationId;
        
        public NetAction(ushort type, ushort location) {
            this.type = type;
            locationId = location;
        }
        
        public void Send() => NetService.Action(type, locationId, 0f, 0f);
        
        public void Send(Vector3 direction) => NetService.Action(type, locationId, direction.x, direction.z);
        
        public bool Equals(NetAction other) => other != null && type == other.type && locationId == other.locationId;
        
        public override bool Equals(object obj) => obj is NetAction other && type == other.type && locationId == other.locationId;
        
        public override int GetHashCode() => HashCode.Combine(type, locationId);
    }
}