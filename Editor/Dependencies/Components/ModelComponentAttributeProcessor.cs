// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TinyMVC.Dependencies.Components;

namespace TinyMVC.Editor.Dependencies.Components {
    public sealed class ModelComponentAttributeProcessor : OdinAttributeProcessor<ModelComponent> {
        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes) {
            attributes.Add(new HideLabelAttribute());
            attributes.Add(new HideReferenceObjectPickerAttribute());
            attributes.Add(new HideDuplicateReferenceBoxAttribute());
            attributes.Add(new ShowInInspectorAttribute());
        }
    }
}