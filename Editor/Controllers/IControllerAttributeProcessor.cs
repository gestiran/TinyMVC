// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TinyMVC.Controllers;

namespace TinyMVC.Editor.Controllers {
    internal sealed class IControllerAttributeProcessor : OdinAttributeProcessor<IController> {
        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes) {
            attributes.Add(new InlinePropertyAttribute());
            attributes.Add(new HideReferenceObjectPickerAttribute());
            attributes.Add(new HideDuplicateReferenceBoxAttribute());
        }
    }
}
#endif