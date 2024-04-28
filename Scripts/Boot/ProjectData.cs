using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TinyMVC.Dependencies;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Boot {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, InlineProperty, HideLabel, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class ProjectData {
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, HideInEditorMode, HideReferenceObjectPicker, HideDuplicateReferenceBox]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowFoldout = false)]
        private List<DependencyContainer> _dependenciesEditor;
    #endif
        
        private DependencyContainer _container;
        
        private readonly Dictionary<int, DependencyContainer> _dependencies;

        private const int _CAPACITY = 16;

        internal ProjectData() {
            _dependencies = new Dictionary<int, DependencyContainer>(_CAPACITY);
        #if ODIN_INSPECTOR && UNITY_EDITOR
            _dependenciesEditor = new List<DependencyContainer>(_CAPACITY);
        #endif
        }

        public void Resolve(List<IResolving> resolving) {
            ResolveUtility.Resolve(resolving, GetContainer());
        }

        public void Resolve([NotNull] IResolving resolving) {
            ResolveUtility.Resolve(resolving, GetContainer());
        }

        public void Resolve([NotNull] params IResolving[] resolving) {
            ResolveUtility.Resolve(resolving, GetContainer());
        }

        public void Resolve([NotNull] DependencyContainer container, List<IResolving> resolving) {
            ResolveUtility.Resolve(resolving, container);
        }

        public void Resolve([NotNull] DependencyContainer container, [NotNull] IResolving resolving) {
            ResolveUtility.Resolve(resolving, container);
        }

        public void Resolve([NotNull] DependencyContainer container, [NotNull] params IResolving[] resolving) {
            ResolveUtility.Resolve(resolving, container);
        }

        internal void ResolveWithoutApply(List<IResolving> resolving) {
            ResolveUtility.ResolveWithoutApply(resolving, GetContainer());
        }

        internal void ResolveWithoutApply([NotNull] IResolving resolving) {
            ResolveUtility.ResolveWithoutApply(resolving, GetContainer());
        }

        internal void ResolveWithoutApply([NotNull] params IResolving[] resolving) {
            ResolveUtility.ResolveWithoutApply(resolving, GetContainer());
        }
        
        internal void ResolveWithoutApply([NotNull] DependencyContainer container, List<IResolving> resolving) {
            ResolveUtility.ResolveWithoutApply(resolving, container);
        }

        internal void ResolveWithoutApply([NotNull] DependencyContainer container, [NotNull] IResolving resolving) {
            ResolveUtility.ResolveWithoutApply(resolving, container);
        }

        internal void ResolveWithoutApply([NotNull] DependencyContainer container, [NotNull] params IResolving[] resolving) {
            ResolveUtility.ResolveWithoutApply(resolving, container);
        }

        internal void Add(int sceneId, DependencyContainer container) {
            if (_dependencies.ContainsKey(sceneId)) {
                _dependencies[sceneId] = container;
            }

            _dependencies.Add(sceneId, container);
            _container = null;
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            RecreateDisplayEditor();
        #endif
        }

        internal void Remove(int sceneId) {
            if (_dependencies.ContainsKey(sceneId) == false) {
                return;
            }

            _dependencies.Remove(sceneId);
            _container = null;
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            RecreateDisplayEditor();
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

    #if ODIN_INSPECTOR && UNITY_EDITOR
        
        private void RecreateDisplayEditor() {
            _dependenciesEditor.Clear();

            foreach (DependencyContainer container in _dependencies.Values) {
                _dependenciesEditor.Add(container);
            }
        }
        
    #endif
        
    }
}