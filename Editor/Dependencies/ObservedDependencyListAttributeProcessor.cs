// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TinyMVC.Dependencies;

namespace TinyMVC.Editor.Dependencies {
    internal sealed class ObservedDependencyListAttributeProcessor<T> : OdinAttributeProcessor<ObservedDependencyList<T>> where T : IDependency {
        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes) {
            attributes.Add(new HideLabelAttribute());
            attributes.Add(new ShowInInspectorAttribute());
            attributes.Add(new HideReferenceObjectPickerAttribute());
            attributes.Add(new HideDuplicateReferenceBoxAttribute());
        }
        
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes) {
            if (member.Name == "list") {
                attributes.Add(new ListDrawerSettingsAttribute() {
                    HideAddButton = true, HideRemoveButton = true, DraggableItems = false, IsReadOnly = true, ListElementLabelName = "@ToString()"
                });
                
                attributes.Add(new ShowInInspectorAttribute());
                attributes.Add(new HideLabelAttribute());
                attributes.Add(new HideInEditorModeAttribute());
                attributes.Add(new HideReferenceObjectPickerAttribute());
                attributes.Add(new HideDuplicateReferenceBoxAttribute());
                attributes.Add(new SearchableAttribute());
            }
        }
    }
}
#endif