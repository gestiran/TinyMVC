using System;
using System.Collections.Generic;
using System.Linq;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Dependencies {
    #if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
    #endif
    public sealed class DependencyContainer {
        #if ODIN_INSPECTOR && UNITY_EDITOR
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowFoldout = false, ListElementLabelName = "@GetType().Name")]
        [ShowInInspector, HideInEditorMode, LabelText("Dependencies"), HideReferenceObjectPicker, HideDuplicateReferenceBox, Searchable]
        internal List<IDependency> display;
        
        #endif
        internal readonly Dictionary<Type, IDependency> dependencies;
        
        internal DependencyContainer(int capacity) => dependencies = new Dictionary<Type, IDependency>(capacity);
        
        public DependencyContainer(List<IDependency> dependencies) : this(dependencies.Count) {
            for (int i = 0; i < dependencies.Count; i++) {
                if (dependencies[i] is Dependency dependency) {
                    Type[] types = dependency.types;
                    
                    for (int typeId = 0; typeId < types.Length; typeId++) {
                        this.dependencies.Add(types[typeId], dependency.link);
                    }
                } else {
                    this.dependencies.Add(dependencies[i].GetType(), dependencies[i]);
                }
            }
            
            #if ODIN_INSPECTOR && UNITY_EDITOR
            display = dependencies;
            #endif
        }
        
        public DependencyContainer(params IDependency[] dependencies) : this(dependencies.Length) {
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
            
            #if ODIN_INSPECTOR && UNITY_EDITOR
            display = dependencies.ToList();
            #endif
        }
        
        public DependencyContainer(IDependency dependency) : this(1) {
            if (dependency is Dependency link) {
                Type[] types = link.types;
                
                for (int typeId = 0; typeId < types.Length; typeId++) {
                    dependencies.Add(types[typeId], link.link);
                }
            } else {
                dependencies.Add(dependency.GetType(), dependency);
            }
            
            #if ODIN_INSPECTOR && UNITY_EDITOR
            display = new List<IDependency>() { dependency };
            #endif
        }
        
        public void Add(IDependency dependency) {
            if (dependency is Dependency link) {
                Type[] types = link.types;
                
                for (int typeId = 0; typeId < types.Length; typeId++) {
                    dependencies.Add(types[typeId], link.link);
                }
            } else {
                dependencies.Add(dependency.GetType(), dependency);
            }
            
            #if ODIN_INSPECTOR && UNITY_EDITOR
            display.Add(dependency);
            #endif
        }
    }
}