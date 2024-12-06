using System;
using System.Collections.Generic;
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
        
        private readonly Dictionary<string, DependencyContainer> _dependencies;
        private readonly Type _resolveContainers = typeof(ResolveGroupAttribute);
        
        public const string MAIN = "Project";
        
        private const int _CAPACITY = 16;
        
        internal ProjectData() {
            _dependencies = new Dictionary<string, DependencyContainer>(_CAPACITY);
        #if UNITY_EDITOR
            _dependenciesEditor = new List<DependencyLink>(_CAPACITY);
        #endif
        }
        
        public bool TryGetDependency<T>(string key, out T dependency) where T : IDependency {
            if (_dependencies.TryGetValue(key, out DependencyContainer container)) {
                if (container.dependencies.TryGetValue(typeof(T), out IDependency value)) {
                    dependency = (T)value;
                    return true;
                }
            }
            
            dependency = default;
            return false;
        }
        
        public bool TryGetDependency(string key, Type type, out IDependency dependency) {
            if (_dependencies.TryGetValue(key, out DependencyContainer container)) {
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
            ResolveGroupAttribute attribute = (ResolveGroupAttribute)Attribute.GetCustomAttribute(type, _resolveContainers);
            string key = attribute != null ? attribute.group : MAIN;
            
            if (_dependencies.TryGetValue(key, out DependencyContainer container) && container.dependencies.TryGetValue(type, out IDependency value)) {
                dependency = (T)value;
                return true;
            }
            
        #if UNITY_EDITOR
            
            Debug.LogError($"Can't find {type.Name} dependency!");
            
        #endif
            
            dependency = default;
            return false;
        }
        
        internal void Remove(string[] groups) {
            foreach (string group in groups) {
                _dependencies.Remove(group);
            }
            
        #if UNITY_EDITOR
            UpdateEditor();
        #endif
        }
        
        internal void Add(List<IDependency> dependencies) {
            foreach (IDependency dependency in dependencies) {
                ResolveGroupAttribute attribute = (ResolveGroupAttribute)Attribute.GetCustomAttribute(dependency.GetType(), _resolveContainers);
                
                string key = attribute != null ? attribute.group : MAIN;
                
                if (_dependencies.TryGetValue(key, out DependencyContainer container)) {
                    container.Update(dependency);
                } else {
                    _dependencies.Add(key, new DependencyContainer(dependency));
                }
            }
            
        #if UNITY_EDITOR
            UpdateEditor();
        #endif
        }
        
    #if UNITY_EDITOR
        
        private void UpdateEditor() {
            _dependenciesEditor = new List<DependencyLink>();
            
            foreach (KeyValuePair<string, DependencyContainer> pair in _dependencies) {
                _dependenciesEditor.Add(new DependencyLink(pair.Key, pair.Value));
            }
        }
        
    #endif
    }
}