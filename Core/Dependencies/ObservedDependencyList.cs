// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TinyReactive;
using TinyReactive.Fields;

namespace TinyMVC.Dependencies {
    public sealed class ObservedDependencyList<T> : ObservedList<T>, IDependency where T : IDependency {
        public ObservedDependencyList(int capacity = Observed.CAPACITY) : base(capacity) { }
        
        public ObservedDependencyList(T[] value, int capacity = Observed.CAPACITY) : base(value, capacity) { }
        
        public ObservedDependencyList([NotNull] DependencyPool<T> pool, int capacity = Observed.CAPACITY) : base(new List<T>(pool), capacity) { }
        
        public ObservedDependencyList([NotNull] params DependencyPool<T>[] pools) : base(CreateListFromPools(pools)) { }
        
        public ObservedDependencyList(List<T> value, int capacity = Observed.CAPACITY) : base(value, capacity) { }
        
        private static List<T> CreateListFromPools(DependencyPool<T>[] pools) {
            int length = 0;
            
            for (int poolId = 0; poolId < pools.Length; poolId++) {
                length += pools[poolId].length;
            }
            
            List<T> result = new List<T>(length);
            
            for (int poolId = 0; poolId < pools.Length; poolId++) {
                result.AddRange(pools[poolId]);
            }
            
            return result;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnAddListenerValue<TV>(ActionListener listener, IUnloadLink unload) where TV : T {
            AddOnAddListener(v =>
            {
                if (v is TV) {
                    listener.Invoke();
                }
            }, unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnAddListenerValue<TV>(ActionListener<TV> listener, IUnloadLink unload) where TV : T {
            AddOnAddListener(v =>
            {
                if (v is TV target) {
                    listener.Invoke(target);
                }
            }, unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnRemoveListenerValue<TV>(ActionListener listener, IUnloadLink unload) where TV : T {
            AddOnRemoveListener(v =>
            {
                if (v is TV) {
                    listener.Invoke();
                }
            }, unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDependencyList<T> AddOnRemoveListenerValue<TV>(ActionListener<TV> listener, IUnloadLink unload) where TV : T {
            AddOnRemoveListener(v =>
            {
                if (v is TV target) {
                    listener.Invoke(target);
                }
            }, unload);
            
            return this;
        }
    }
}