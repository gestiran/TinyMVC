// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using TinyMVC.Views;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies.Components {
    public abstract class Actor<T> : Actor where T : View {
    #if ODIN_INSPECTOR
        [field: ShowInInspector, LabelText("View"), HideReferenceObjectPicker, HideDuplicateReferenceBox, PropertyOrder(-10000), ReadOnly]
    #endif
        public new T view { get; internal set; }
        
        internal override View viewInternal { get => view; set => view = value as T; }
        
        private static readonly Type _viewType = typeof(T);
        
        public override Type GetTypeView() => _viewType;
        
    #if UNITY_EDITOR
        
        internal override bool isVisibleView => false;
        
    #endif
    }
    
    public abstract class Actor : Model {
        public View view { get => viewInternal; internal set => viewInternal = value; }
        
    #if ODIN_INSPECTOR
        [field: ShowInInspector, LabelText("View"), HideReferenceObjectPicker, HideDuplicateReferenceBox, PropertyOrder(-10000), ShowIf("isVisibleView"), ReadOnly]
    #endif
        internal virtual View viewInternal { get; set; }
        
        public virtual Type GetTypeView() => typeof(View);
        
    #if UNITY_EDITOR
        
        internal virtual bool isVisibleView => true;
        
    #endif
    }
}