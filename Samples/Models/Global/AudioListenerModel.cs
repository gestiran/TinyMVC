// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Samples.Models.Positions;
using TinyReactive.Fields;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Models.Global {
    public class AudioListenerModel : IPosition {
#if ODIN_INSPECTOR
        [ShowInInspector]
 #endif
        public Observed<Vector3> position { get; set; }
        
        public Observed<bool> isActive;
        public Observed<AudioVelocityUpdateMode> velocityUpdateMode;
    }
}