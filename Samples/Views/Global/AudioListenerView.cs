using Sirenix.OdinInspector;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;

namespace TinyMVC.Samples.Views.Global {
    [RequireComponent(typeof(AudioListener))]
    [DisallowMultipleComponent]
    public class AudioListenerView : View, IApplyResolving, IApplyGenerated, IUnload, IDontDestroyOnLoad, IDependency {
        public bool isActive => thisAudioListener.enabled;
        public Vector3 position => thisTransform.position;
        public AudioVelocityUpdateMode velocityUpdateMode => thisAudioListener.velocityUpdateMode;
        
        [field: SerializeField, FoldoutGroup("Generated"), Required]
        public AudioListener thisAudioListener { get; private set; }
        
        [field: SerializeField, FoldoutGroup("Generated"), Required]
        public Transform thisTransform { get; private set; }
        
        [Inject] protected AudioListenerModel _model;
        
        public virtual void ApplyResolving() {
            ChangePosition(_model.position.value);
            ChangeActiveState(_model.isActive.value);
            ChangeVelocityUpdateMode(_model.velocityUpdateMode.value);
            
            _model.position.AddListener(ChangePosition);
            _model.isActive.AddListener(ChangeActiveState);
            _model.velocityUpdateMode.AddListener(ChangeVelocityUpdateMode);
        }
        
        public virtual void Unload() {
            _model.position.RemoveListener(ChangePosition);
            _model.isActive.RemoveListener(ChangeActiveState);
            _model.velocityUpdateMode.RemoveListener(ChangeVelocityUpdateMode);
        }
        
        private void ChangePosition(Vector3 newPosition) => thisTransform.position = newPosition;
        
        private void ChangeActiveState(bool state) => thisAudioListener.enabled = state;
        
        private void ChangeVelocityUpdateMode(AudioVelocityUpdateMode mode) => thisAudioListener.velocityUpdateMode = mode;
        
    #if UNITY_EDITOR
        
        [ContextMenu("Soft reset")]
        public override void Reset() {
            thisAudioListener = GetComponent<AudioListener>();
            thisTransform = transform;
            
            base.Reset();
        }
        
    #endif
    }
}