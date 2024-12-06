using Sirenix.OdinInspector;
using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Models.Positions;
using UnityEngine;

namespace TinyMVC.Samples.Models.Global {
    [ResolveGroup(Dependencies.SERVICES)]
    public sealed class AudioListenerModel : IPosition {
        [ShowInInspector] public Observed<Vector3> position { get; set; }
        
        public Observed<bool> isActive;
        public Observed<AudioVelocityUpdateMode> velocityUpdateMode;
    }
}