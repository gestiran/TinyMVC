// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Boot;
using TinyMVC.Boot.Binding;
using TinyMVC.Samples.Models.Global;
using TinyMVC.Samples.Views.Global;
using TinyReactive.Fields;

namespace TinyMVC.Samples.Binding.Global {
    public sealed class EventSystemBind : Binder<EventSystemModel> {
        private readonly EventSystemView _view;
        
        public EventSystemBind() {
            ProjectContext.data.Get(out _view);
        }
        
        public EventSystemBind(EventSystemView view) {
            _view = view;
        }
        
        protected override void Bind(EventSystemModel model) {
            model.isActive = new Observed<bool>(_view.isActive);
        }
    }
}