using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Functions;
using UnityEngine;

namespace TinyMVC.Samples.Models {
    public sealed class MainCameraModel : IPosition, IDependency {
        public Observed<Vector3> position { get; set; }
        
        public Camera camera;

    #if URP_RENDER_PIPELINE
        public InputListener<Camera> addToStack;
    #endif
    }
}