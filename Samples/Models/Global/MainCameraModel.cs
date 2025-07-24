// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Models.Positions;
using UnityEngine;

namespace TinyMVC.Samples.Models.Global {
    public sealed class MainCameraModel : IPosition {
        public Observed<Vector3> position { get; set; }
        
        public Camera camera;

    #if URP_RENDER_PIPELINE
        public InputListener<Camera> addToStack;
    #endif
    }
}