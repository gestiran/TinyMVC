using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Functions;
using UnityEngine;

namespace TinyMVC.Samples.Models {
    public sealed class AudioListenerModel : IPosition, IDependency {
        public Observed<Vector3> position { get; set; }
        
        public Observed<bool> isActive;
        public Observed<AudioVelocityUpdateMode> velocityUpdateMode;
    }
}