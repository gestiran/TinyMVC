// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Boot;
using TinyMVC.Boot.Binding;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Samples.Views.Global;
using TinyReactive.Fields;
using UnityEngine;

namespace TinyMVC.Samples.Binding.Global {
    public sealed class AudioListenerBind : Binder<AudioListenerModel> {
        private readonly AudioListenerView _view;
        
        public AudioListenerBind() {
            ProjectContext.data.Get(out _view);
        }
        
        public AudioListenerBind(AudioListenerView view) {
            _view = view;
        }
        
        protected override void Bind(AudioListenerModel model) {
            model.position = new Observed<Vector3>(_view.position);
            
            model.isActive = new Observed<bool>(_view.isActive);
            model.velocityUpdateMode = new Observed<AudioVelocityUpdateMode>(_view.velocityUpdateMode);
        }
    }
}