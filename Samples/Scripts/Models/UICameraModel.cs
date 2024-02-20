using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Functions;
using UnityEngine;

namespace TinyMVC.Samples.Models {
    public sealed class UICameraModel : IPosition, IDependency {
        public Observed<Vector3> position { get; set; }
        
        public Camera camera;
    }
}