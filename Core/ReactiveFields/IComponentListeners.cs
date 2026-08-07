// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyMVC.Dependencies.Components;

namespace TinyMVC.ReactiveFields {
    internal interface IComponentListeners {
        public void TryInvokeAdd(Model model, ModelComponent component);
        
        public void TryInvokeRemove(Model model, ModelComponent component);
    }
}