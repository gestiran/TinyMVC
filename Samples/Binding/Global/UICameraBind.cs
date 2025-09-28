// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Boot;
using TinyMVC.Boot.Binding;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Samples.Views.Global;
using TinyReactive.Fields;
using UnityEngine;

namespace TinyMVC.Samples.Binding.Global {
    public sealed class UICameraBind : Binder<UICameraModel> {
        private readonly UICameraView _view;
        
        public UICameraBind() {
            ProjectContext.data.Get(out _view);
        }
        
        public UICameraBind(UICameraView view) {
            _view = view;
        }

        protected override void Bind(UICameraModel model) {
            model.position = new Observed<Vector3>(_view.position);
            
            model.camera = _view.thisCamera;
        }
    }
}