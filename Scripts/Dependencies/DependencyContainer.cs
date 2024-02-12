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
    internal sealed class DependencyContainer {
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowFoldout = false, ListElementLabelName = "@GetType().Name")]
        [ShowInInspector, HideInEditorMode, LabelText("Dependencies"), HideReferenceObjectPicker, HideDuplicateReferenceBox, Searchable]
        internal List<IDependency> display;
        
    #endif
        internal readonly Dictionary<Type, IDependency> dependencies;

        internal DependencyContainer(int capacity) => dependencies = new Dictionary<Type, IDependency>(capacity);

        internal DependencyContainer(List<IDependency> dependencies) : this(dependencies.Count) {
            for (int i = 0; i < dependencies.Count; i++) {
                this.dependencies.Add(dependencies[i].GetType(), dependencies[i]);
            }
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            display = dependencies;
        #endif
        }
        
        internal DependencyContainer(IDependency[] dependencies) : this(dependencies.Length) {
            for (int i = 0; i < dependencies.Length; i++) {
                this.dependencies.Add(dependencies[i].GetType(), dependencies[i]);
            }
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            display = dependencies.ToList();
        #endif
        }
        
        internal DependencyContainer(IDependency dependency) : this(1) {
            dependencies.Add(dependency.GetType(), dependency);
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            display = new List<IDependency>() { dependency };
        #endif
        }
    }
}