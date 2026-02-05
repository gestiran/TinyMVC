// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Reflection;
using TinyMVC.Boot.Binding;
using TinyMVC.Dependencies;
using TinyReactive;
using TinyReactive.Extensions;
using UnityEngine;
using Binder = TinyMVC.Boot.Binding.Binder;

namespace TinyMVC.Boot.Contexts {
    public abstract class ModelsContext {
        protected UnloadPool _unload { get; private set; }
        
        internal string key;
        
        internal readonly List<IDependency> dependenciesBinded;
        internal readonly List<IDependency> dependencies;
        
        public sealed class EmptyContext : ModelsContext {
            internal EmptyContext() { }
            
            protected override void Bind() { }
            
            protected override void Create(List<IDependency> _) { }
        }
        
        protected ModelsContext() {
            dependenciesBinded = new List<IDependency>();
            dependencies = new List<IDependency>();
        }
        
        public static EmptyContext Empty() => new EmptyContext();
        
        internal void ConnectUnload(UnloadPool unload) => _unload = unload;
        
        internal void CreateBinders(string contextKey) {
            key = contextKey;
            Bind();
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            for (int i = 0; i < assemblies.Length; i++) {
                foreach (RegisterBinderAttribute attribute in assemblies[i].GetCustomAttributes<RegisterBinderAttribute>()) {
                    Debug.LogError($"Create: {attribute.binderType.Name} :: {attribute.priority}");
                }
            }
        }
        
        internal void Create() => Create(dependencies);
        
        internal void Unload() {
            dependenciesBinded.TryUnload();
            dependencies.TryUnload();
        }
        
        protected void Add(Binder binder) {
            if (binder is IBindConditions conditions && conditions.IsNeedBinding() == false) {
                return;
            }
            
            binder.ConnectUnload(_unload);
            IDependency dependency = binder.GetDependency();
            ProjectContext.data.Add(key, dependency);
            dependenciesBinded.Add(dependency);
        }
        
        protected void Add(BinderSystem binderSystem) {
            if (binderSystem is IBindConditions conditions && conditions.IsNeedBinding() == false) {
                return;
            }
            
            binderSystem.ConnectUnload(_unload);
            binderSystem.Connect(this);
        }
        
        protected void Add(BinderMod binder) {
            if (binder is IBindConditions conditions && conditions.IsNeedBinding() == false) {
                return;
            }
            
            binder.ConnectUnload(_unload);
            binder.BindInternal();
        }
        
        protected abstract void Bind();
        
        protected virtual void Create(List<IDependency> models) { }
    }
}