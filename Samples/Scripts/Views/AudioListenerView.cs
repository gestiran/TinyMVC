using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;
using TinyMVC.Samples.Models;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Views {
    [RequireComponent(typeof(AudioListener))]
    [DisallowMultipleComponent]
    public sealed class AudioListenerView : View, IApplyResolving, IDependency, IApplyGenerated, IDontDestroyOnLoad {
        public bool isActive => thisAudioListener.enabled;
        public Vector3 position => thisTransform.position;

        public AudioVelocityUpdateMode velocityUpdateMode => thisAudioListener.velocityUpdateMode;

        [field: SerializeField
    #if UNITY_EDITOR && ODIN_INSPECTOR
        , FoldoutGroup("Generated"), Required, ReadOnly
    #endif
        ]
        public AudioListener thisAudioListener { get; private set; }
        
        [field: SerializeField
    #if UNITY_EDITOR && ODIN_INSPECTOR
        , FoldoutGroup("Generated"), Required, ReadOnly
    #endif
        ]
        public Transform thisTransform { get; private set; }

        [Inject] private AudioListenerModel _model;

        public void ApplyResolving() {
            ProjectContext.current.TryGetGlobalUnload(out UnloadPool unload);
            
            ChangePosition(_model.position.value);
            ChangeActiveState(_model.isActive.value);
            ChangeVelocityUpdateMode(_model.velocityUpdateMode.value);

            _model.position.AddListener(ChangePosition, unload);
            _model.isActive.AddListener(ChangeActiveState, unload);
            _model.velocityUpdateMode.AddListener(ChangeVelocityUpdateMode, unload);
        }
        
        private void ChangePosition(Vector3 newPosition) => thisTransform.position = newPosition;
        
        private void ChangeActiveState(bool state) => thisAudioListener.enabled = state;
        
        private void ChangeVelocityUpdateMode(AudioVelocityUpdateMode mode) => thisAudioListener.velocityUpdateMode = mode;

    #if UNITY_EDITOR
        
        [ContextMenu("Soft reset")]
        public void Reset() {
            thisAudioListener = GetComponent<AudioListener>();
            thisTransform = transform;
        }
        
    #endif
    }
}