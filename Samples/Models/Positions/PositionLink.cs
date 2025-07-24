using TinyMVC.ReactiveFields;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Models.Positions {
    public sealed class PositionLink : IPosition {
    #if ODIN_INSPECTOR
        [ShowInInspector, HideLabel]
    #endif
        public Observed<Vector3> position { get; set; }
        
        public PositionLink(Observed<Vector3> position) => this.position = position;
        
        public PositionLink(Vector3 position) => this.position = new Observed<Vector3>(position);
    }
}