using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TinyMVC.Dependencies;
using TinyMVC.Entities;

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
        private int _targetEntityId;
        
        private readonly Dictionary<int, DependencyContainer> _dependencies;
        private readonly Dictionary<int, List<Entity>> _entities;
        private readonly Dictionary<Type, IData> _singleEntities;

        public const int DEFAULT_GROUP = -1;
        private const int _CAPACITY = 16;
        private const int _SINGLE_CAPACITY = 32;

        internal ProjectData() {
            _targetEntityId = 0;
            _dependencies = new Dictionary<int, DependencyContainer>(_CAPACITY);
            _entities = new Dictionary<int, List<Entity>>(_CAPACITY);
            _singleEntities = new Dictionary<Type, IData>(_SINGLE_CAPACITY);
        #if ODIN_INSPECTOR && UNITY_EDITOR
            _dependenciesEditor = new List<DependencyContainer>(_CAPACITY);
        #endif
        }

        public Entity AddEntity(int id = DEFAULT_GROUP) {
            Entity entity = new Entity(_targetEntityId++, id);
            
            if (_entities.TryGetValue(id, out List<Entity> entities)) {
                entities.Add(entity);
            } else {
                _entities.Add(id, new List<Entity>() { entity });
            }

            return entity;
        }
        
        public void RemoveEntity(Entity entity) {
            if (_entities.TryGetValue(entity.groupId, out List<Entity> entities)) {
                entities.Remove(entity);
            }
        }

        public void SetSingle<T>(T component) where T : IData => _singleEntities.Add(typeof(T), component);
        
        public void RemoveSingle<T>() where T : IData => _singleEntities.Remove(typeof(T));

        public bool TryGetSingle<T>(out T component) where T : IData {
            if (_singleEntities.TryGetValue(typeof(T), out IData componentData)) {
                component = (T)componentData;
                return true;
            }
            
            component = default;
            return false;
        }

        public IEnumerable<T> ForEach<T>() where T : IData {
            foreach (List<Entity> entities in _entities.Values) {
                foreach (Entity entity in entities) {
                    foreach (T data in entity.ForEach<T>()) {
                        yield return data;
                    }
                }
            }
        }
        
        public IEnumerable<(Entity, T)> ForEachEntities<T>() where T : IData {
            foreach (List<Entity> entities in _entities.Values) {
                foreach (Entity entity in entities) {
                    if (entity.TryGetEntity(out (Entity, T) data)) {
                        yield return data;
                    }
                }
            }
        }
        
        public IEnumerable<(Entity, T1, T2)> ForEachEntities<T1, T2>() where T1 : IData where T2 : IData {
            foreach (List<Entity> entities in _entities.Values) {
                foreach (Entity entity in entities) {
                    if (entity.TryGetEntity(out (Entity, T1, T2) data)) {
                        yield return data;
                    }
                }
            }
        }
        
        public IEnumerable<(Entity, T1, T2, T3)> ForEachEntities<T1, T2, T3>() where T1 : IData where T2 : IData where T3 : IData {
            foreach (List<Entity> entities in _entities.Values) {
                foreach (Entity entity in entities) {
                    if (entity.TryGetEntity(out (Entity, T1, T2, T3) data)) {
                        yield return data;
                    }
                }
            }
        }
        
        public IEnumerable<(Entity, T1, T2, T3, T4)> ForEachEntities<T1, T2, T3, T4>() where T1 : IData where T2 : IData where T3 : IData where T4 : IData {
            foreach (List<Entity> entities in _entities.Values) {
                foreach (Entity entity in entities) {
                    if (entity.TryGetEntity(out (Entity, T1, T2, T3, T4) data)) {
                        yield return data;
                    }
                }
            }
        }
        
        public IEnumerable<(T1, T2)> ForEach<T1, T2>() where T1 : IData where T2 : IData {
            foreach (List<Entity> entities in _entities.Values) {
                foreach (Entity entity in entities) {
                    foreach ((T1, T2) data in entity.ForEach<T1, T2>()) {
                        yield return data;
                    }
                }
            }
        }
        
        public IEnumerable<(T1, T2, T3)> ForEach<T1, T2, T3>() where T1 : IData where T2 : IData where T3 : IData {
            foreach (List<Entity> entities in _entities.Values) {
                foreach (Entity entity in entities) {
                    foreach ((T1, T2, T3) data in entity.ForEach<T1, T2, T3>()) {
                        yield return data;
                    }
                }
            }
        }
        
        public IEnumerable<(T1, T2, T3, T4)> ForEach<T1, T2, T3, T4>() where T1 : IData where T2 : IData where T3 : IData where T4 : IData {
            foreach (List<Entity> entities in _entities.Values) {
                foreach (Entity entity in entities) {
                    foreach ((T1, T2, T3, T4) data in entity.ForEach<T1, T2, T3, T4>()) {
                        yield return data;
                    }
                }
            }
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