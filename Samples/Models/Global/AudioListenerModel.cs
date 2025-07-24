using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Models.Positions;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Models.Global {
    public sealed class AudioListenerModel : IPosition {
#if ODIN_INSPECTOR
        [ShowInInspector]
 #endif
        public Observed<Vector3> position { get; set; }
        
        public Observed<bool> isActive;
        public Observed<AudioVelocityUpdateMode> velocityUpdateMode;
    }
}