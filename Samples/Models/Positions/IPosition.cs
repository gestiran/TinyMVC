using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;
using UnityEngine;

namespace TinyMVC.Samples.Models.Positions {
    public interface IPosition : IDependency {
        public Observed<Vector3> position { get; }
    }
}