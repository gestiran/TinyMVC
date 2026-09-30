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
                    ObservedDrawerSettingsAttribute settings = Property.GetAttribute<ObservedDrawerSettingsAttribute>();
                    
                    if (settings != null) {
                        if (settings.ShowButtons) {
                            if (current is ObservedSave<int> observedInt) {
                                if (ObservedDrawer.DrawValueAndButtonsInt(observedInt, label)) {
                                    ValueEntry.Values.ForceMarkDirty();
                                }
                            } else if (current is ObservedSave<float> observedFloat) {
                                if (ObservedDrawer.DrawValueAndButtonsFloat(observedFloat, label)) {
                                    ValueEntry.Values.ForceMarkDirty();
                                }
                            } else {
                                DrawValue(label, valueProperty, current);
                            }
                        } else {
                            DrawValue(label, valueProperty, current);
                        }
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