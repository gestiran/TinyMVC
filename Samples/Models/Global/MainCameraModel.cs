// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Samples.Models.Positions;
using TinyReactive.Fields;
using UnityEngine;

namespace TinyMVC.Samples.Models.Global {
    public class MainCameraModel : IPosition {
        public Observed<Vector3> position { get; set; }
        
        public Camera camera;

    #if URP_RENDER_PIPELINE
        public InputListener<Camera> addToStack;
    #endif
    }
}