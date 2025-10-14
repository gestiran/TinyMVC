// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Views.Global {
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public class UICameraView : View, IApplyResolving, IDependency, IApplyGenerated, IDontDestroyOnLoad {
        public Vector3 position => thisTransform.position;
        
    #if ODIN_INSPECTOR
        [field: BoxGroup("Links"), ChildGameObjectsOnly(IncludeInactive = true), Required]
    #endif
        [field: SerializeField]
        public Camera inputCamera { get; private set; }
        
    #if ODIN_INSPECTOR
        [field: FoldoutGroup("Generated", 1000), Required, ReadOnly]
    #endif
        [field: SerializeField]
        public Transform thisTransform { get; private set; }
        
    #if ODIN_INSPECTOR
        [field: FoldoutGroup("Generated", 1000), Required, ReadOnly]
    #endif
        [field: SerializeField]
        public Camera thisCamera { get; private set; }
        
        private MainCameraModel _mainCamera;
        
        public void ApplyResolving() {
            ProjectContext.data.Get(out _mainCamera);
            
        #if URP_RENDER_PIPELINE
            _mainCamera.addToStack.Send(inputCamera, thisCamera);
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