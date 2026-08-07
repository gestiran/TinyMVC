// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TinyMVC.Boot.Helpers;

namespace TinyMVC.Editor.Boot.Helpers {
    internal sealed class ContextLinkAttributeProcessor<T> : OdinAttributeProcessor<ContextLink<T>> {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes) {
            if (member.Name == "contextKey") {
                attributes.Add(new HideInEditorModeAttribute());
                attributes.Add(new HideInPlayModeAttribute());
            } else if (member.Name == "context") {
                attributes.Add(new ShowInInspectorAttribute());
                attributes.Add(new TitleAttribute("@contextKey"));
                attributes.Add(new InlinePropertyAttribute());
                attributes.Add(new HideLabelAttribute());
                attributes.Add(new HideInEditorModeAttribute());
                attributes.Add(new HideReferenceObjectPickerAttribute());
                attributes.Add(new HideDuplicateReferenceBoxAttribute());
            }
        }
    }
}