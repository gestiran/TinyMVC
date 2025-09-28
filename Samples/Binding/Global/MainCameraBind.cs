// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Boot;
using TinyMVC.Boot.Binding;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Samples.Views.Global;
using TinyReactive.Fields;
using UnityEngine;

namespace TinyMVC.Samples.Binding.Global {
    public sealed class MainCameraBind : Binder<MainCameraModel> {
        private readonly MainCameraView _view;
        
        public MainCameraBind() {
            ProjectContext.data.Get(out _view);
        }
        
        public MainCameraBind(MainCameraView view) {
            _view = view;
        }
        
        protected override void Bind(MainCameraModel model) {
            model.position = new Observed<Vector3>(_view.position);
            
            model.camera = _view.thisCamera;
            
        #if URP_RENDER_PIPELINE
            model.addToStack = new InputListener<Camera>();
        #endif
        }
    }
}