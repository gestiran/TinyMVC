// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

#if UNITY_EDITOR
#define DEBUG
#endif

using System;
using System.Collections.Generic;
using TinyReactive.Fields;

#if DEBUG
using TinyUtilities.Logger;
#endif

namespace TinyMVC.Dependencies {
    public sealed class DependencyContainer : IDisposable {
        internal readonly Dictionary<Type, IDependency> dependencies;
        
        internal static DependencyContainer empty { get; }
        
    #if DEBUG
        internal readonly InputListener<Type, IDependency> onUpdate;
    #endif
        
        static DependencyContainer() {
            empty = new DependencyContainer(0);
        }
        
        private DependencyContainer(int capacity) {
            dependencies = new Dictionary<Type, IDependency>(capacity);
            
        #if DEBUG
            onUpdate = new InputListener<Type, IDependency>();
        #endif
        }
        
        internal DependencyContainer(ICollection<IDependency> dependencies) : this(dependencies.Count) {
            foreach (IDependency dependency in dependencies) {
            #if DEBUG
                
                if (dependency == null) {
                    DebugUtility.LogError("Can't load!");
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
        }
        
        public void Dispose() {
        #if DEBUG
            onUpdate.Unload();
        #endif
        }
        
        internal void Update(IDependency dependency) {
            if (dependency is Dependency link) {
                Type[] types = link.types;
                
                for (int typeId = 0; typeId < types.Length; typeId++) {
                    dependencies[types[typeId]] = link.link;
                    
                #if DEBUG
                    onUpdate.Send(types[typeId], dependency);
                #endif
                }
            } else {
                Type type = dependency.GetType();
                dependencies[type] = dependency;
                
            #if DEBUG
                onUpdate.Send(type, dependency);
            #endif
            }
        }
    }
}