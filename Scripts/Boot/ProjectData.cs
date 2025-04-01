using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyMVC.Dependencies;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TinyMVC.Boot {
    [ShowInInspector, InlineProperty, HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox]
    public sealed class ProjectData {
    #if UNITY_EDITOR
        [ShowInInspector, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowFoldout = false, ListElementLabelName = "@label")]
        private List<DependencyLink> _dependenciesEditor;
        
        [HideReferenceObjectPicker, HideDuplicateReferenceBox]
        private sealed class DependencyLink {
            [HideLabel, ShowInInspector, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox]
            private DependencyContainer _container;
            
            [HideInInspector] public readonly string label;
            
            public DependencyLink(string label, DependencyContainer container) {
                this.label = label;
                _container = container;
            }
        }
        
    #endif
        
        internal DependencyContainer tempContainer;
        private readonly Dictionary<string, DependencyContainer> _dependencies;
        
        public const string MAIN = "Project";
        
        private const int _CAPACITY = 16;
        
        internal ProjectData() {
            _dependencies = new Dictionary<string, DependencyContainer>(_CAPACITY);
        #if UNITY_EDITOR
            _dependenciesEditor = new List<DependencyLink>(_CAPACITY);
        #endif
        }
        
        public bool TryGetDependency<T>(out T dependency) where T : IDependency {
            Type type = typeof(T);
            
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = (T)tempValue;
                return true;
            }
            
            foreach (DependencyContainer container in _dependencies.Values) {
                if (container.dependencies.TryGetValue(typeof(T), out IDependency value)) {
                    dependency = (T)value;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public bool TryGetDependency<T>(string contextKey, out T dependency) where T : IDependency {
            Type type = typeof(T);
            
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = (T)tempValue;
                return true;
            } 
            
            if (_dependencies.TryGetValue(contextKey, out DependencyContainer container)) {
                if (container.dependencies.TryGetValue(typeof(T), out IDependency value)) {
                    dependency = (T)value;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public bool TryGetDependency(string contextKey, Type type, out IDependency dependency) {
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = tempValue;
                return true;
            } 
            
            if (_dependencies.TryGetValue(contextKey, out DependencyContainer container)) {
                if (container.dependencies.TryGetValue(type, out IDependency value)) {
                    dependency = value;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public bool TryGetDependency(Type type, out IDependency dependency) {
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = tempValue;
                return true;
            }
            
            foreach (DependencyContainer container in _dependencies.Values) {
                if (container.dependencies.TryGetValue(type, out IDependency value)) {
                    dependency = value;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public bool Get<T>(out T dependency) where T : IDependency {
            Type type = typeof(T);
            
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = (T)tempValue;
                return true;
            }
            
            foreach (DependencyContainer container in _dependencies.Values) {
                if (container.dependencies.TryGetValue(type, out IDependency value)) {
                    dependency = (T)value;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public bool Get<T>(string contextKey, out T dependency) where T : IDependency {
            Type type = typeof(T);
            
            if (tempContainer != null && tempContainer.dependencies.TryGetValue(type, out IDependency tempValue)) {
                dependency = (T)tempValue;
                return true;
            } 
            
            if (_dependencies.TryGetValue(contextKey, out DependencyContainer container) && container.dependencies.TryGetValue(type, out IDependency value)) {
                dependency = (T)value;
                return true;
            }
            
            dependency = default;
            return false;
        }
        
        internal void Add(string contextKey, List<IDependency> dependencies) {
            foreach (IDependency dependency in dependencies) {
                AddDependency(contextKey, dependency);
            }
            
        #if UNITY_EDITOR
            UpdateEditor();
        #endif
        }
        
        internal void Add(string contextKey, IDependency dependency) {
            AddDependency(contextKey, dependency);
            
        #if UNITY_EDITOR
            UpdateEditor();
        #endif
        }
        
        internal void Remove(string contextKey) {
            _dependencies.Remove(contextKey);
            
        #if UNITY_EDITOR
            UpdateEditor();
        #endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddDependency(string contextKey, IDependency dependency) {
            if (_dependencies.TryGetValue(contextKey, out DependencyContainer container)) {
                container.Update(dependency);
            } else {
                _dependencies.Add(contextKey, new DependencyContainer(dependency));
            }
        }
        
    #if UNITY_EDITOR
        
        private void UpdateEditor() {
            _dependenciesEditor.Clear();
            
            foreach (KeyValuePair<string, DependencyContainer> pair in _dependencies) {
                _dependenciesEditor.Add(new DependencyLink(pair.Key, pair.Value));
            }
        }
        
    #endif
    }
}