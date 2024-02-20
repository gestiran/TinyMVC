using TinyMVC.ReactiveFields;
using UnityEngine;

namespace TinyMVC.Samples.Functions {
    public interface IPosition {
        public Observed<Vector3> position { get; }
    }
}