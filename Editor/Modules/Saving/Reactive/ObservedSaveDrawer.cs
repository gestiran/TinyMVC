// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using Sirenix.OdinInspector.Editor;
using TinyMVC.Modules.Saving.Reactive;
using TinyReactive.Editor.Fields;
using UnityEditor;
using UnityEngine;

namespace TinyMVC.Editor.Modules.Saving.Reactive {
    [DrawerPriority(0, 10, 1)]
    public sealed class ObservedSaveDrawer<T> : OdinValueDrawer<ObservedSave<T>> {
        protected override void DrawPropertyLayout(GUIContent label) {
            ObservedSave<T> current = ValueEntry.SmartValue;
            
            if (current != null) {
                InspectorProperty valueProperty = Property.Children[ObservedDrawer.VALUE];
                
                if (valueProperty == null && Property.Children.Count > 0) {
                    valueProperty = Property.Children[0];
                }
                
                if (valueProperty != null) {
                    if (current is ObservedSave<int> observedInt) {
                        EditorGUILayout.BeginHorizontal();
                        
                        DrawValue(label, valueProperty, current);
                        
                        if (ObservedDrawer.DrawButtonsInt(observedInt, GUILayout.Width(64f))) {
                            ValueEntry.Values.ForceMarkDirty();
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    } else if (current is ObservedSave<float> observedFloat) {
                        EditorGUILayout.BeginHorizontal();
                        
                        DrawValue(label, valueProperty, current);
                        
                        if (ObservedDrawer.DrawButtonsFloat(observedFloat, GUILayout.Width(64f))) {
                            ValueEntry.Values.ForceMarkDirty();
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    } else {
                        DrawValue(label, valueProperty, current);
                    }
                }
                
                return;
            }
            
            CallNextDrawer(label);
        }
        
        private void DrawValue(GUIContent label, InspectorProperty property, ObservedSave<T> current) {
            EditorGUI.BeginChangeCheck();
            property.Draw(label);
            
            if (EditorGUI.EndChangeCheck() && property.ValueEntry.WeakSmartValue is T newValue) {
                current.Set(newValue);
                ValueEntry.Values.ForceMarkDirty();
            }
        }
    }
}