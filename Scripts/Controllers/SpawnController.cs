// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using JetBrains.Annotations;
using TinyMVC.Boot;
using TinyMVC.Views;

namespace TinyMVC.Controllers {
    public abstract class SpawnController : IController {
        protected T Spawn<T>([NotNull] T view) where T : View => Spawn(view, ProjectContext.scene.key);
        
        protected T Spawn<T>([NotNull] T view, string contextKey) where T : View {
            if (view.connectState == View.ConnectState.Connected) {
                return view;
            }
            
            if (ProjectContext.TryGetContext(contextKey, out SceneContext context)) {
                view.connectState = View.ConnectState.Connected;
                context.Connect(view);
            }
            
            return view;
        }
    }
}