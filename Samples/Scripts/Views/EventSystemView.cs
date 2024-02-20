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
    public sealed class EventSystemView : View, IInit, IApplyResolving, IUnload, IDependency, IApplyGenerated, IDontDestroyOnLoad {
        public bool isActive => thisEventSystem.enabled;

        [field: SerializeField
    #if UNITY_EDITOR && ODIN_INSPECTOR
        , FoldoutGroup("Generated"), Required, ReadOnly
    #endif
        ]
        public EventSystem thisEventSystem { get; private set; }

        private UnloadPool _unload;

        public void Init() => _unload = new UnloadPool();

        [Inject] private EventSystemModel _model;

        public void ApplyResolving() {
            ChangeActiveState(_model.isActive.value);
            
            _model.isActive.AddListener(ChangeActiveState, _unload);
        }

        public void Unload() => _unload.Unload();

        private void ChangeActiveState(bool state) => thisEventSystem.enabled = state;

        [ContextMenu("Soft reset")]
        public void Reset() => thisEventSystem = GetComponent<EventSystem>();
    }
}