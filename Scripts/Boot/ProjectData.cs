using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TinyMVC.Dependencies;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
#endif

namespace TinyMVC.Boot {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, InlineProperty, HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class ProjectData {
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox, Searchable]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowFoldout = false, ListElementLabelName = "@label")]
        private List<DependencyLink> _dependenciesEditor;
        
        [HideReferenceObjectPicker, HideDuplicateReferenceBox]
        private sealed class DependencyLink {
            [HideLabel, ShowInInspector, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox]
            private DependencyContainer _container;
            
            [HideInInspector]
            public readonly string label;
            
            public DependencyLink(string label, DependencyContainer container) {
                this.label = label;
                _container = container;
            }
        }
        
    #endif
        
        private DependencyContainer _container;
        
        private readonly Dictionary<int, DependencyContainer> _dependencies;
        
        private const int _CAPACITY = 16;
        
        internal ProjectData() {
            _dependencies = new Dictionary<int, DependencyContainer>(_CAPACITY);
        #if ODIN_INSPECTOR && UNITY_EDITOR
            _dependenciesEditor = new List<DependencyLink>(_CAPACITY);
        #endif
        }
        
        public void Resolve(List<IResolving> resolving) {
            ResolveUtility.Resolve(resolving, GetContainer());
        }
        
        public void Resolve([NotNull] IResolving resolving) {
            ResolveUtility.Resolve(resolving, GetContainer());
        }
        
        public void Resolve([NotNull] DependencyContainer container, [NotNull] IResolving resolving) {
            ResolveUtility.Resolve(resolving, container);
        }
        
        internal void ResolveWithoutApply(List<IResolving> resolving) {
            ResolveUtility.ResolveWithoutApply(resolving, GetContainer());
        }
        
        internal void ResolveWithoutApply([NotNull] IResolving resolving) {
            ResolveUtility.ResolveWithoutApply(resolving, GetContainer());
        }
        
        internal void ResolveWithoutApply([NotNull] DependencyContainer container, List<IResolving> resolving) {
            ResolveUtility.ResolveWithoutApply(resolving, container);
        }
        
        internal void ResolveWithoutApply([NotNull] DependencyContainer container, [NotNull] IResolving resolving) {
            ResolveUtility.ResolveWithoutApply(resolving, container);
        }
        
        internal void Add(int sceneId, DependencyContainer container) {
            if (_dependencies.ContainsKey(sceneId)) {
                _dependencies[sceneId] = container;
            }
            
            _dependencies.Add(sceneId, container);
            _container = null;
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            _dependenciesEditor.Add(new DependencyLink(SceneManager.GetSceneByBuildIndex(sceneId).name, container));
        #endif
        }
        
        internal void Remove(int sceneId) {
            if (_dependencies.ContainsKey(sceneId) == false) {
                return;
            }
            
            _dependencies.Remove(sceneId);
            _container = null;
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            string name = SceneManager.GetSceneByBuildIndex(sceneId).name;
            
            for (int linkId = 0; linkId < _dependenciesEditor.Count; linkId++) {
                if (_dependenciesEditor[linkId].label == name) {
                    _dependenciesEditor.RemoveAt(linkId);
                    break;
                }
            }
        #endif
        }
        
        private DependencyContainer GetContainer() {
            if (_container != null) {
                return _container;
            }
            
            DependencyContainer result = new DependencyContainer(64);
            
            foreach (DependencyContainer container in _dependencies.Values) {
                foreach (KeyValuePair<Type, IDependency> dependencies in container.dependencies) {
                    result.dependencies.Add(dependencies.Key, dependencies.Value);
                }
            }
            
            _container = result;
            
            return result;
        }
    }
}