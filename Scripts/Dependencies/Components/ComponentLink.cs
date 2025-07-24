// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Dependencies.Components {
    public sealed class ComponentLink<T> {
        public readonly Model model;
        public readonly T component;
        
        internal ComponentLink(Model model, T component) {
            this.model = model;
            this.component = component;
        }
    }
}