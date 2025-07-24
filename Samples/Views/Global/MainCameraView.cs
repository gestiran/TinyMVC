// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if URP_RENDER_PIPELINE
using UnityEngine.Rendering.Universal;
#endif

namespace TinyMVC.Samples.Views.Global {
#if URP_RENDER_PIPELINE
    [RequireComponent(typeof(UniversalAdditionalCameraData))]
#endif
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class MainCameraView : View, IApplyResolving, IUnload, IDependency, IApplyGenerated, IDontDestroyOnLoad {
        public Vector3 position => thisTransform.position;
        
    #if ODIN_INSPECTOR
        [field: FoldoutGroup("Generated"), Required, ReadOnly]
    #endif
        [field: SerializeField]
        public Camera thisCamera { get; private set; }
        
    #if ODIN_INSPECTOR
        [field: FoldoutGroup("Generated"), Required, ReadOnly]
    #endif
        [field: SerializeField]
        public Transform thisTransform { get; private set; }
        
    #if URP_RENDER_PIPELINE
        [field: SerializeField, FoldoutGroup("Generated"), Required, ReadOnly]
        public UniversalAdditionalCameraData thisCameraData { get; private set; }
    #endif
        
        private MainCameraModel _model;
        
        public void ApplyResolving() {
            ProjectContext.data.Get(out _model);
            
            ChangePosition(_model.position.value);
            
            _model.position.AddListener(ChangePosition);
            
        #if URP_RENDER_PIPELINE
            _model.addToStack.AddListener(AddToStack);
        #endif
        }
        
        public void Unload() {
            _model.position.RemoveListener(ChangePosition);
            
        #if URP_RENDER_PIPELINE
            _model.addToStack.RemoveListener(AddToStack);
        #endif
        }
        
        private void ChangePosition(Vector3 newPosition) => thisTransform.position = newPosition;
        
    #if URP_RENDER_PIPELINE
        private void AddToStack(Camera cam) => thisCameraData.cameraStack.Add(cam);
    #endif
        
    #if UNITY_EDITOR
        
        [ContextMenu("Soft reset")]
        public override void Reset() {
            thisCamera = GetComponent<Camera>();
            thisTransform = transform;
            
        #if URP_RENDER_PIPELINE
            thisCameraData = GetComponent<UniversalAdditionalCameraData>();
        #endif
            
            base.Reset();
        }
        
    #endif
    }
}