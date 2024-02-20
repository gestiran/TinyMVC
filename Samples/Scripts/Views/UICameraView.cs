using TinyMVC.Dependencies;
using TinyMVC.Samples.Models;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Views {
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class UICameraView : View, IApplyResolving, IDependency, IApplyGenerated, IDontDestroyOnLoad {
        public Vector3 position => thisTransform.position;
        
        [field: SerializeField
    #if UNITY_EDITOR && ODIN_INSPECTOR
        , BoxGroup("Links"), ChildGameObjectsOnly(IncludeInactive = true), Required
    #endif
        ]
        public Camera inputCamera { get; private set; }
        
        [field: SerializeField
    #if UNITY_EDITOR && ODIN_INSPECTOR
        , FoldoutGroup("Generated"), Required, ReadOnly
    #endif
        ]
        public Transform thisTransform { get; private set; }
        
    [field: SerializeField
    #if UNITY_EDITOR && ODIN_INSPECTOR
        , FoldoutGroup("Generated"), Required, ReadOnly
    #endif
        ]
        public Camera thisCamera { get; private set; }

        [Inject] private MainCameraModel _mainCamera;

        public void ApplyResolving() {
        #if URP_RENDER_PIPELINE
            _mainCamera.addToStack.Send(inputCamera, thisCamera);
        #endif
        }

        [ContextMenu("Soft reset")]
        public void Reset() {
            thisTransform = transform;
            thisCamera = GetComponent<Camera>();
        }
    }
}