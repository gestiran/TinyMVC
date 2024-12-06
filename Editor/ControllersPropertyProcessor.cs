using System;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using TinyMVC.Boot;
using TinyMVC.Controllers;
using TinyMVC.Loop;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TinyMVC.Editor {
    public sealed class ControllersPropertyProcessor<T> : OdinValueDrawer<T> where T : IController {
        private string _label;
        
        private bool _isTick;
        private bool _tickToggle;
        
        private bool _isFixedTick;
        private bool _fixedTickToggle;
        
        private bool _isLateTick;
        private bool _lateTickToggle;
        
        private FieldInfo _loopContext;
        
        protected override void Initialize() {
            Type type = typeof(T);
            
            _label = GetLabel(type);
            
            _isTick = type.GetInterface(nameof(ITick)) != null;
            
            if (_isTick) {
                _tickToggle = true;
            }
            
            _isFixedTick = type.GetInterface(nameof(IFixedTick)) != null;
            
            if (_isFixedTick) {
                _fixedTickToggle = true;
            }
            
            _isLateTick = type.GetInterface(nameof(ILateTick)) != null;
            
            if (_isLateTick) {
                _lateTickToggle = true;
            }
            
            _loopContext = typeof(ProjectContext).GetField("_loopContext", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        
        protected override void DrawPropertyLayout(GUIContent label) {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginHorizontal("Box");
            
            GUI.enabled = _isTick;
            LoopToggle<ITick>(ref _tickToggle, "ConnectTick", "DisconnectTick");
            
            GUI.enabled = _isFixedTick;
            LoopToggle<IFixedTick>(ref _fixedTickToggle, "ConnectFixedTick", "DisconnectFixedTick");
            
            GUI.enabled = _isLateTick;
            LoopToggle<ILateTick>(ref _lateTickToggle, "ConnectLateTick", "DisconnectLateTick");
            
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Label(_label);
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void LoopToggle<TLoop>(ref bool value, string connect, string disconnect) where TLoop : ILoop {
            bool temp = GUILayout.Toggle(value, "", GUILayout.Width(14));
            
            if (temp != value) {
                if (ValueEntry.SmartValue is not TLoop loop) {
                    return;
                }
                
                if (temp) {
                    MethodInfo method = _loopContext.FieldType.GetMethod(connect, BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    if (method == null) {
                        return;
                    }
                    
                    method.Invoke(_loopContext.GetValue(null), new object[] { SceneManager.GetActiveScene().buildIndex, loop });
                } else {
                    MethodInfo method = _loopContext.FieldType.GetMethod(disconnect, BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    if (method == null) {
                        return;
                    }
                    
                    method.Invoke(_loopContext.GetValue(null), new object[] { SceneManager.GetActiveScene().buildIndex, loop });
                }
                
                value = temp;
            }
        }
        
        private string GetLabel(Type type) {
            string label = type.Name;
            
            if (type.IsGenericType) {
                Type[] arguments = type.GenericTypeArguments;
                
                for (int i = 0; i < arguments.Length; i++) {
                    label = label.Replace($"`{i + 1}", $"<{arguments[i].Name}>");
                }
            }
            
            return label;
        }
    }
}