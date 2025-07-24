// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using TinyMVC.ReactiveFields;
using UnityEngine;
#endif

namespace TinyMVC.Dependencies {
    public sealed class DependencyContainer : IDisposable {
        internal readonly Dictionary<Type, IDependency> dependencies;
        
        internal static DependencyContainer empty { get; }
        
    #if UNITY_EDITOR
        internal readonly InputListener<Type, IDependency> onUpdate;
    #endif
        
        static DependencyContainer() {
            empty = new DependencyContainer(0);
        }
        
        private DependencyContainer(int capacity) {
            dependencies = new Dictionary<Type, IDependency>(capacity);
            
        #if UNITY_EDITOR
            onUpdate = new InputListener<Type, IDependency>();
        #endif
        }
        
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
        #if UNITY_EDITOR
            onUpdate.Unload();
        #endif
        }
        
        internal void Update(IDependency dependency) {
            if (dependency is Dependency link) {
                Type[] types = link.types;
                
                for (int typeId = 0; typeId < types.Length; typeId++) {
                    dependencies[types[typeId]] = link.link;
                    
                #if UNITY_EDITOR
                    onUpdate.Send(types[typeId], dependency);
                #endif
                }
            } else {
                Type type = dependency.GetType();
                dependencies[type] = dependency;
                
            #if UNITY_EDITOR
                onUpdate.Send(type, dependency);
            #endif
            }
        }
    }
}