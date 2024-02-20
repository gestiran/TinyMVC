using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Models;
using TinyMVC.Samples.Views;
using UnityEngine;

namespace TinyMVC.Samples.Binding {
    public sealed class UICameraBind : Binder<UICameraModel> {
        [Inject] private UICameraView _view;

        protected override void Bind(UICameraModel model) {
            model.position = new Observed<Vector3>(_view.position);
            
            model.camera = _view.thisCamera;
        }
    }
}