using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TinyMVC.Boot;
using TinyMVC.Dependencies;
using TinyMVC.Modules.Saving;
using UnityEditor;
using UnityEngine;

namespace TinyMVC.Editor {
    internal sealed class ProjectContextWindow : OdinEditorWindow {
        private bool _isVisibleContexts => _contexts != null && EditorApplication.isPlaying;
        private bool _isVisibleFavorites => _favorites != null && _favorites.Count > 0 && EditorApplication.isPlaying;
        
        [Searchable(FuzzySearch = true, Recursive = false, FilterOptions = SearchFilterOptions.TypeOfValue | SearchFilterOptions.ValueToString)]
        [ShowInInspector, HideReferenceObjectPicker, HideDuplicateReferenceBox, ShowIf("_isVisibleFavorites"), Title("Favorites:")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ListElementLabelName = "@ToString()")]
        private static List<DependencyLink> _favorites;
        
        [ShowInInspector, HideReferenceObjectPicker, HideDuplicateReferenceBox, ShowIf("_isVisibleContexts"), Title("All:")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowFoldout = false)]
        private static List<ContextLink> _contexts;
        
        [ShowInInspector, ReadOnly, HideLabel, HideInPlayMode, HideIf("_isVisibleContexts")]
        private static string _label = "Active only in PlayMode";
        
        private static SaveHandler _save;
        
        [HideReferenceObjectPicker, HideDuplicateReferenceBox]
        private sealed class ContextLink : IEquatable<ContextLink>, IDisposable {
            [Searchable(FuzzySearch = true, Recursive = false, FilterOptions = SearchFilterOptions.TypeOfValue | SearchFilterOptions.ValueToString)]
            [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false)]
            [ShowInInspector, HideInEditorMode, LabelText("@ToString()"), HideReferenceObjectPicker, HideDuplicateReferenceBox]
            private List<DependencyLink> _dependencies;
            
            // ReSharper disable once Unity.RedundantHideInInspectorAttribute
            [HideInInspector] public readonly string key;
            
            public ContextLink(string key, DependencyContainer container) {
                this.key = key;
                _dependencies = ConvertToLinks(container.dependencies);
                
                container.onUpdate.AddListener(Update);
            }
            
            private void Update(Type dependencyType, IDependency dependency) {
                string dependencyKey = ConvertToKey(dependencyType);
                
                for (int dependencyId = 0; dependencyId < _dependencies.Count; dependencyId++) {
                    if (_dependencies[dependencyId].key != dependencyKey) {
                        continue;
                    }
                    
                    _dependencies[dependencyId].value = dependency;
                    return;
                }
                
                _dependencies.Add(new DependencyLink(dependencyKey, dependency));
            }
            
            private List<DependencyLink> ConvertToLinks(Dictionary<Type, IDependency> dependencies) {
                List<DependencyLink> result = new List<DependencyLink>(dependencies.Count);
                
                foreach (KeyValuePair<Type, IDependency> pair in dependencies) {
                    result.Add(new DependencyLink(ConvertToKey(pair.Key), pair.Value));
                }
                
                return result;
            }
            
            public void Dispose() {
                for (int dependencyId = 0; dependencyId < _dependencies.Count; dependencyId++) {
                    _dependencies[dependencyId].Dispose();
                }
            }
            
            private string ConvertToKey(Type type) {
                StringBuilder result = new StringBuilder();
                
                result.Append(type.Namespace);
                
                string name = type.Name;
                
                Type[] arguments = type.GenericTypeArguments;
                
                for (int i = 0; i < arguments.Length; i++) {
                    name = name.Replace($"`{i + 1}", $".{arguments[i].Name}");
                }
                
                result.Append(".");
                result.Append(name);
                
                return result.ToString();
            }
            
            public override string ToString() => key;
            
            public bool Equals(ContextLink other) => other != null && key == other.key;
            
            public override bool Equals(object obj) => obj is ContextLink other && key == other.key;
            
            public override int GetHashCode() => key.GetHashCode();
        }
        
        [ShowInInspector, HideReferenceObjectPicker, HideDuplicateReferenceBox, InlineProperty, HideLabel]
        private sealed class DependencyLink : IDisposable {
            [CustomContextMenu("Add to Favorites", "AddFavorites")]
            [CustomContextMenu("Remove from Favorites", "RemoveFavorites")]
            [ShowInInspector, LabelText("@ToString()")]
            public IDependency value;
            
            private bool _isFavorite;
            
            // ReSharper disable once Unity.RedundantHideInInspectorAttribute
            [HideInInspector] public readonly string key;
            
            public DependencyLink(string key, IDependency dependency) {
                value = dependency;
                this.key = key;
                
                _isFavorite = _save.Has(key);
                
                if (_isFavorite) {
                    _favorites.Add(this);
                }
            }
            
            private void AddFavorites() {
                if (_isFavorite) {
                    return;
                }
                
                _favorites.Add(this);
                _save.Save(true, key);
                _isFavorite = true;
            }
            
            private void RemoveFavorites() {
                if (_isFavorite == false) {
                    return;
                }
                
                _favorites.Remove(this);
                _save.Delete(key);
                _isFavorite = false;
            }
            
            public void Dispose() {
                if (_isFavorite) {
                    _favorites.Remove(this);   
                }
            }
            
            public override string ToString() {
                string title = value.ToString();
                
                if (title.Contains('.')) {
                    title = title.Split('.')[^1].Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "");
                }
                
                return title;
            }
        }
        
        [MenuItem("Window/TinyMVC/ProjectContext", priority = 0)]
        private static void OpenWindow() => GetWindow<ProjectContextWindow>("ProjectContext").Show();
        
        [OnInspectorInit]
        public void Init() {
            _favorites = new List<DependencyLink>();
            _contexts = new List<ContextLink>();
            _save = new SaveHandler("Editor", "V1");
            
            if (EditorApplication.isPlaying) {
                FillContexts();   
            }
            
            ProjectData.onAdd += AddContext;
            ProjectData.onRemove += RemoveContext;
            EditorApplication.playModeStateChanged += StateChange;
            
            _save.Start();
        }
        
        [OnInspectorDispose]
        public void Dispose() {
            _save.Stop();
            
            ProjectData.onAdd -= AddContext;
            ProjectData.onRemove -= RemoveContext;
            EditorApplication.playModeStateChanged -= StateChange;
            
            _favorites = null;
            _contexts = null;
            _save = null;
        }
        
        private void FillContexts() {
            if (ProjectContext.data == null) {
                return;
            }
            
            foreach (KeyValuePair<string, DependencyContainer> pair in ProjectContext.data.contexts) {
                AddContext(pair.Key, pair.Value);
            }
        }
        
        private void AddContext(string contextKey, DependencyContainer container) {
            _contexts.Add(new ContextLink(contextKey, container));
        }
        
        private void RemoveContext(string contextKey) {
            for (int contextId = 0; contextId < _contexts.Count; contextId++) {
                if (_contexts[contextId].key != contextKey) {
                    continue;
                }
                
                _contexts[contextId].Dispose();
                _contexts.RemoveAt(contextId);
                return;
            }
        }
        
        private void StateChange(PlayModeStateChange state) {
            if (state != PlayModeStateChange.ExitingPlayMode) {
                return;
            }
            
            for (int contextId = 0; contextId < _contexts.Count; contextId++) {
                _contexts[contextId].Dispose();
            }
            
            _contexts.Clear();
        }
    }
}