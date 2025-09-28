// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyReactive.Fields;
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