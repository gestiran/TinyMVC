// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TinyMVC.Dependencies;

namespace TinyMVC.Editor.Dependencies {
    internal sealed class IDependencyAttributeProcessor : OdinAttributeProcessor<IDependency> {
        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes) {
            attributes.Add(new HideReferenceObjectPickerAttribute());
            attributes.Add(new HideDuplicateReferenceBoxAttribute());
        }
    }
}
#endif