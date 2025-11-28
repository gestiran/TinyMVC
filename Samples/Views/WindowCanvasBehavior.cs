using Sirenix.OdinInspector;
using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.Samples.Models.Global;
using TinyServices.Windows;
using UnityEngine;

namespace TinyMVC.Samples.Views {
    [RequireComponent(typeof(Canvas))]
    public abstract class WindowCanvasBehavior : WindowBehavior, IApplyResolving, IBeginPlay {
        [field: SerializeField, BoxGroup("Generated"), Required, ReadOnly]
        public Canvas thisCanvas { get; private set; }
        
        protected UICameraModel _uiCamera;
        
        public virtual void ApplyResolving() => ProjectContext.data.Get(out _uiCamera);
        
        public virtual void BeginPlay() => thisCanvas.worldCamera = _uiCamera.camera;
        
    #if UNITY_EDITOR
        
        [ContextMenu("Soft Reset")]
        public override void Reset() {
            thisCanvas = GetComponent<Canvas>();
            base.Reset();
        }
        
    #endif
    }
}