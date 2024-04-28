using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Loop;
using TinyMVC.ReactiveFields.Extensions;
using TinyMVC.Samples.Models;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Views {
    [RequireComponent(typeof(EventSystem))]
    [DisallowMultipleComponent]
    public sealed class EventSystemView : View, IApplyResolving, IDependency, IApplyGenerated, IDontDestroyOnLoad {
        public bool isActive => thisEventSystem.enabled;

        [field: SerializeField
    #if UNITY_EDITOR && ODIN_INSPECTOR
        , FoldoutGroup("Generated"), Required, ReadOnly
    #endif
        ]
        public EventSystem thisEventSystem { get; private set; }

        [Inject] private EventSystemModel _model;

        public void ApplyResolving() {
            ProjectContext.current.TryGetGlobalUnload(out UnloadPool unload);
            
            ChangeActiveState(_model.isActive.value);
            
            _model.isActive.AddListener(ChangeActiveState, unload);
        }

        private void ChangeActiveState(bool state) => thisEventSystem.enabled = state;

    #if UNITY_EDITOR
        
        [ContextMenu("Soft reset")]
        public void Reset() => thisEventSystem = GetComponent<EventSystem>();
        
    #endif
    }
}