// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;

#if URP_RENDER_PIPELINE
using TinyMVC.Samples.Models.Global;
#endif

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Views.Global {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class UICameraView : View, IApplyResolving, IDependency, IApplyGenerated, IDontDestroyOnLoad {
        public Vector3 position => thisTransform.position;
        
    #if URP_RENDER_PIPELINE
        [SerializeField]
        protected bool _autoAddToStack;
    #endif
        
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
        
        protected MainCameraModel _camera;
        
        public virtual void ApplyResolving() {
            ProjectContext.data.Get(out _camera);
            
        #if URP_RENDER_PIPELINE
            if (_autoAddToStack) {
                _camera.addToStack.Send(thisCamera);
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