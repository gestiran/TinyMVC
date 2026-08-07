// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using TinyMVC.Dependencies;
using TinyUtilities.Logger;

namespace TinyMVC.Boot.Contexts {
    public abstract class ParametersContext {
        internal readonly List<IDependency> all;
        
        public sealed class EmptyContext : ParametersContext {
            internal EmptyContext() { }
            
            protected override void Create() { }
        }
        
        protected ParametersContext() => all = new List<IDependency>();
        
        public static EmptyContext Empty() => new EmptyContext();
        
        internal void Init() => Create();
        
        internal void AddDependencies(List<IDependency> dependencies) => dependencies.AddRange(all);
        
        protected abstract void Create();
        
        protected void Add<T>(T dependency) where T : IDependency {
            if (dependency == null) {
                DebugUtility.LogError($"Can't find {typeof(T).Name} parameter");
                return;
            }
            
            all.Add(dependency);
        }
    }
}