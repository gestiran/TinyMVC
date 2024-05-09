using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;
using TinyMVC.Samples.Models;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;

#if URP_RENDER_PIPELINE
using UnityEngine.Rendering.Universal;
#endif

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Views {
#if URP_RENDER_PIPELINE
    [RequireComponent(typeof(UniversalAdditionalCameraData))]
#endif
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class MainCameraView : View, IApplyResolving, IDependency, IApplyGenerated, IDontDestroyOnLoad {
        public Vector3 position => thisTransform.position;
        
        [field: SerializeField
    #if UNITY_EDITOR && ODIN_INSPECTOR
        , FoldoutGroup("Generated"), Required, ReadOnly
    #endif
        ]
        public Camera thisCamera { get; private set; }
        
        [field: SerializeField
    #if UNITY_EDITOR && ODIN_INSPECTOR
        , FoldoutGroup("Generated"), Required, ReadOnly
    #endif
        ]
        public Transform thisTransform { get; private set; }

    #if URP_RENDER_PIPELINE
        [field: SerializeField
    #if UNITY_EDITOR && ODIN_INSPECTOR
        , FoldoutGroup("Generated"), Required, ReadOnly
    #endif
        ]
        public UniversalAdditionalCameraData thisCameraData { get; private set; }
    #endif
        
        [Inject] private MainCameraModel _model;

        public void ApplyResolving() {
            ProjectContext.current.TryGetGlobalUnload(out UnloadPool unload);
            
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
        public void Reset() {
            thisCamera = GetComponent<Camera>();
            thisTransform = transform;
            
        #if URP_RENDER_PIPELINE
            thisCameraData = GetComponent<UniversalAdditionalCameraData>();
        #endif
        }
        
    #endif
    }
}