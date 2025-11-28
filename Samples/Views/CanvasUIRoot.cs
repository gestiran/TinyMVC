// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

#if TINY_SERVICES_WINDOWS
    using TinyMVC.Boot;
    using TinyMVC.Dependencies;
    using TinyMVC.Samples.Models.Global;
    using TinyServices.Windows;
    
    namespace TinyMVC.Samples.Views {
        public sealed class CanvasUIRoot : CanvasWindowsRoot, IApplyResolving {
            private UICameraModel _cameraUI;
            
            public void ApplyResolving() => ProjectContext.data.Get(out _cameraUI);
            
            public override void BeginPlay() {
                canvas.worldCamera = _cameraUI.camera;
                base.BeginPlay();
            }
        }
    }
#endif