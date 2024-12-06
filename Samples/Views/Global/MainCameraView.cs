#if URP_RENDER_PIPELINE
using UnityEngine.Rendering.Universal;
#endif
    
    using Sirenix.OdinInspector;
    using TinyMVC.Boot;
    using TinyMVC.Dependencies;
    using TinyMVC.Loop;
    using TinyMVC.ReactiveFields;
    using TinyMVC.Samples.Models.Global;
    using TinyMVC.Views;
    using TinyMVC.Views.Generated;
    using UnityEngine;
    
    namespace TinyMVC.Samples.Views.Global {
#if URP_RENDER_PIPELINE
    [RequireComponent(typeof(UniversalAdditionalCameraData))]
#endif
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class MainCameraView : View, IApplyResolving, IDependency, IApplyGenerated, IDontDestroyOnLoad {
        public Vector3 position => thisTransform.position;
        
        [field: SerializeField, FoldoutGroup("Generated"), Required, ReadOnly]
        public Camera thisCamera { get; private set; }
        
        [field: SerializeField, FoldoutGroup("Generated"), Required, ReadOnly]
        public Transform thisTransform { get; private set; }

    #if URP_RENDER_PIPELINE
        [field: SerializeField, FoldoutGroup("Generated"), Required, ReadOnly]
        public UniversalAdditionalCameraData thisCameraData { get; private set; }
    #endif
        
        [Inject] private MainCameraModel _model;

        public void ApplyResolving() {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            
            ChangePosition(_model.position.value);
            
            _model.position.AddListener(ChangePosition, unload);
            
        #if URP_RENDER_PIPELINE
            _model.addToStack.AddListener(AddToStack, unload);
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