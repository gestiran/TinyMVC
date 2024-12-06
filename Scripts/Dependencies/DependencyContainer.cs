using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
#endif

namespace TinyMVC.Dependencies {
#if UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class DependencyContainer {
    #if UNITY_EDITOR
        [Searchable(FuzzySearch = false, Recursive = false)]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowFoldout = false, ListElementLabelName = "@GetType().Name")]
        [ShowInInspector, HideInEditorMode, LabelText("Dependencies"), HideReferenceObjectPicker, HideDuplicateReferenceBox]
        internal List<IDependency> display;
    #endif
        internal readonly Dictionary<Type, IDependency> dependencies;
        
        internal static DependencyContainer empty { get; }
        
        static DependencyContainer() => empty = new DependencyContainer(0);
        
        internal DependencyContainer(int capacity) => dependencies = new Dictionary<Type, IDependency>(capacity);
        
        internal DependencyContainer(ICollection<IDependency> dependencies) : this(dependencies.Count) {
            foreach (IDependency dependency in dependencies) {
            #if UNITY_EDITOR
                
                if (dependency == null) {
                    Debug.LogError("Can't load!");
                    continue;
                }
                
            #endif
                
                if (dependency is Dependency other) {
                    Type[] types = other.types;
                    
                    for (int typeId = 0; typeId < types.Length; typeId++) {
                        this.dependencies.Add(types[typeId], other.link);
                    }
                } else {
                    this.dependencies.Add(dependency.GetType(), dependency);
                }
            }
            
        #if UNITY_EDITOR
            display = new List<IDependency>();
            display.AddRange(dependencies);
        #endif
        }
        
        internal DependencyContainer(params IDependency[] dependencies) : this(dependencies.Length) {
            for (int i = 0; i < dependencies.Length; i++) {
                if (dependencies[i] is Dependency dependency) {
                    Type[] types = dependency.types;
                    
                    for (int typeId = 0; typeId < types.Length; typeId++) {
                        this.dependencies.Add(types[typeId], dependency.link);
                    }
                } else {
                    this.dependencies.Add(dependencies[i].GetType(), dependencies[i]);
                }
            }
            
        #if UNITY_EDITOR
            display = dependencies.ToList();
        #endif
        }
        
        internal DependencyContainer(IDependency dependency) : this(1) {
            if (dependency is Dependency link) {
                Type[] types = link.types;
                
                for (int typeId = 0; typeId < types.Length; typeId++) {
                    dependencies.Add(types[typeId], link.link);
                }
            } else {
                dependencies.Add(dependency.GetType(), dependency);
            }
            
        #if UNITY_EDITOR
            display = new List<IDependency>() { dependency };
        #endif
        }
        
        internal void Update(IDependency dependency) {
            if (dependency is Dependency link) {
                Type[] types = link.types;
                
                for (int typeId = 0; typeId < types.Length; typeId++) {
                    dependencies[types[typeId]] = link.link;
                }
            } else {
                dependencies[dependency.GetType()] = dependency;
            }
            
        #if UNITY_EDITOR
            display = new List<IDependency>();
            display.AddRange(dependencies.Values);
        #endif
        }
    }
}