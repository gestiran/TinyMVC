// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Dependencies;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;

#if URP_RENDER_PIPELINE
using TinyMVC.Boot;
using TinyMVC.Samples.Models.Global;
#endif

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Views.Global {
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class UICameraView : View, IApplyResolving, IDependency, IApplyGenerated, IDontDestroyOnLoad {
        public Vector3 position => thisTransform.position;
        
    #if ODIN_INSPECTOR
        [field: ChildGameObjectsOnly(IncludeInactive = true), Required]
    #endif
        [field: SerializeField]
        public Camera inputCamera { get; private set; }
        
    #if ODIN_INSPECTOR
        [field: BoxGroup("Generated"), Required, ReadOnly]
    #endif
        [field: SerializeField]
        public Transform thisTransform { get; private set; }
        
    #if ODIN_INSPECTOR
        [field: BoxGroup("Generated"), Required, ReadOnly]
    #endif
        [field: SerializeField]
        public Camera thisCamera { get; private set; }
        
        public void ApplyResolving() {
        #if URP_RENDER_PIPELINE
            if (ProjectContext.data.Get(out MainCameraModel mainCamera)) {
                mainCamera.addToStack.Send(inputCamera, thisCamera);
            }
        #endif
        }
        
    #if UNITY_EDITOR
        
        [ContextMenu("Soft reset")]
        public override void Reset() {
            thisTransform = transform;
            thisCamera = GetComponent<Camera>();
            
            base.Reset();
        }
        
    #endif
    }
}