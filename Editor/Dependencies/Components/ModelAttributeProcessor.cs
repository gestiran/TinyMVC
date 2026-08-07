// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TinyMVC.Dependencies.Components;

namespace TinyMVC.Editor.Dependencies.Components {
    internal sealed class ModelAttributeProcessor : OdinAttributeProcessor<Model> {
        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes) {
            attributes.Add(new HideReferenceObjectPickerAttribute());
            attributes.Add(new HideDuplicateReferenceBoxAttribute());
        }
        
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes) {
            if (member.Name == "components") {
                attributes.Add(new ShowInInspectorAttribute());
                attributes.Add(new HideLabelAttribute());
            }
        }
    }
}