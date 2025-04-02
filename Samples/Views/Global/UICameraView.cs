using Sirenix.OdinInspector;
using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;

namespace TinyMVC.Samples.Views.Global {
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class UICameraView : View, IApplyResolving, IDependency, IApplyGenerated, IDontDestroyOnLoad {
        public Vector3 position => thisTransform.position;
        
        [field: SerializeField, BoxGroup("Links"), ChildGameObjectsOnly(IncludeInactive = true), Required]
        public Camera inputCamera { get; private set; }
        
        [field: SerializeField, FoldoutGroup("Generated", 1000), Required, ReadOnly]
        public Transform thisTransform { get; private set; }
        
        [field: SerializeField, FoldoutGroup("Generated", 1000), Required, ReadOnly]
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