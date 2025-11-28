// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using UnityEngine;

#if URP_RENDER_PIPELINE
using TinyMVC.Samples.Models.Global;
#endif

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Views.Global {
    public class UIInputView : UICameraView {
    #if ODIN_INSPECTOR
        [field: ChildGameObjectsOnly(IncludeInactive = true), Required]
    #endif
        [field: SerializeField]
        public Camera inputCamera { get; private set; }
        
        public override void ApplyResolving() {
            base.ApplyResolving();
            
        #if URP_RENDER_PIPELINE
            if (_autoAddToStack) {
                _camera.addToStack.Send(inputCamera);
            }
        #endif
        }
    }
}