// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TinyMVC.Boot.Contexts;

namespace TinyMVC.Editor.Boot.Contexts {
    internal sealed class ControllersContextAttributeProcessor : OdinAttributeProcessor<ControllersContext> {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes) {
            if (member.Name == "systems") {
                attributes.Add(new ShowInInspectorAttribute());
            } else if (member.Name == "_controllers") {
                attributes.Add(new DictionaryDrawerSettings() {
                    DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Group", ValueLabel = "Controllers"
                });
                
                attributes.Add(new ShowInInspectorAttribute());
            }
        }
    }
}
#endif