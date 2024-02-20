using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Models;
using TinyMVC.Samples.Views;
using UnityEngine;

namespace TinyMVC.Samples.Binding {
    public sealed class AudioListenerBind : Binder<AudioListenerModel> {
        [Inject] private AudioListenerView _view;
        
        protected override void Bind(AudioListenerModel model) {
            model.position = new Observed<Vector3>(_view.position);
            
            model.isActive = new Observed<bool>(_view.isActive);
            model.velocityUpdateMode = new Observed<AudioVelocityUpdateMode>(_view.velocityUpdateMode);
        }
    }
}