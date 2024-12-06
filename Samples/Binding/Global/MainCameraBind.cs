using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyMVC.ReactiveFields;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Samples.Views.Global;
using UnityEngine;

namespace TinyMVC.Samples.Binding.Global {
    public sealed class MainCameraBind : Binder<MainCameraModel> {
        [Inject] private MainCameraView _view;

        protected override void Bind(MainCameraModel model) {
            model.position = new Observed<Vector3>(_view.position);
            
            model.camera = _view.thisCamera;

        #if URP_RENDER_PIPELINE
            model.addToStack = new InputListener<Camera>();
        #endif
        }
    }
}