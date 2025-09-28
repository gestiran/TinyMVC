// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Views;
using TinyMVC.Views.Generated;
using TinyReactive;
using UnityEngine;
using UnityEngine.EventSystems;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Samples.Views.Global {
    [RequireComponent(typeof(EventSystem))]
    [DisallowMultipleComponent]
    public sealed class EventSystemView : View, IApplyResolving, IDependency, IApplyGenerated, IUnload, IDontDestroyOnLoad {
        public bool isActive => thisEventSystem.enabled;
        
    #if ODIN_INSPECTOR
        [field: FoldoutGroup("Generated"), Required, ReadOnly]
    #endif
        [field: SerializeField]
        public EventSystem thisEventSystem { get; private set; }
        
        private EventSystemModel _model;
        
        public void ApplyResolving() {
            ProjectContext.data.Get(out _model);
            
            ChangeActiveState(_model.isActive.value);
            
            _model.isActive.AddListener(ChangeActiveState);
        }
        
        public void Unload() {
            _model.isActive.RemoveListener(ChangeActiveState);
        }
        
        private void ChangeActiveState(bool state) => thisEventSystem.enabled = state;
        
    #if UNITY_EDITOR
        
        [ContextMenu("Soft reset")]
        public override void Reset() {
            thisEventSystem = GetComponent<EventSystem>();
            base.Reset();
        }
        
    #endif
    }
}