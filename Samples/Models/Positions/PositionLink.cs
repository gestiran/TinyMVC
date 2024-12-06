using Sirenix.OdinInspector;
using TinyMVC.ReactiveFields;
using UnityEngine;

namespace TinyMVC.Samples.Models.Positions {
    public sealed class PositionLink : IPosition {
        [ShowInInspector, HideLabel]
        public Observed<Vector3> position { get; set; }
        
        public PositionLink(Observed<Vector3> position) => this.position = position;
        
        public PositionLink(Vector3 position) => this.position = new Observed<Vector3>(position);
    }
}