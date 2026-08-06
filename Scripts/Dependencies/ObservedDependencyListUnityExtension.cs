// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using TinyUtilities;
using UnityEngine;

#if UNITASK_ENABLE
using Task = Cysharp.Threading.Tasks.UniTask;
#else
using Task = System.Threading.Tasks.Task;
#endif

namespace TinyMVC.Dependencies {
    public static class ObservedDependencyListUnityExtension {
        /// <summary> Maximum allowable ANR when performing asynchronous operations. </summary>
        public static int asyncAnrMS { get; private set; }
        
        /// <summary> Locking multiple requests to a single object. </summary>
        private static readonly Dictionary<int, bool> _lock;
        
        static ObservedDependencyListUnityExtension() {
            _lock = new Dictionary<int, bool>(32);
            asyncAnrMS = 64;
        }
        
        /// <summary> Set the allowable ANR value when performing asynchronous operations. </summary>
        /// <param name="ms"> Value in milliseconds. </param>
        public static void OverrideDefaultANR(int ms) => asyncAnrMS = ms;
        
        public static Task AddAsync<T>(this ObservedDependencyList<T> current, [NotNull] params T[] values) where T : IDependency {
            return current.AddAsync(asyncAnrMS, AsyncUtility.token, values);
        }
        
        public static Task AddAsync<T>(this ObservedDependencyList<T> current, CancellationToken cancellation, [NotNull] params T[] values)
            where T : IDependency {
            return current.AddAsync(asyncAnrMS, cancellation, values);
        }
        
        public static async Task AddAsync<T>(this ObservedDependencyList<T> current, int anr, CancellationToken cancellation, [NotNull] params T[] values)
            where T : IDependency {
            if (_lock.TryAdd(current.id, true) == false) {
            #if UNITY_EDITOR || PERFORMANCE_DEBUG
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }
            
            current.list.AddRange(values);
            DateTime now = DateTime.Now;
            
            for (int i = current.onAdd.Count - 1; i >= 0; i--) {
                current.onAdd[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            for (int i = current.onAddWithValue.Count - 1; i >= 0; i--) {
                for (int j = 0; j < values.Length; j++) {
                    current.onAddWithValue[i].Invoke(values[j]);
                }
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
        
        public static Task AddAsync<T>(this ObservedDependencyList<T> current, [NotNull] T value) where T : IDependency {
            return current.AddAsync(asyncAnrMS, AsyncUtility.token, value);
        }
        
        public static Task AddAsync<T>(this ObservedDependencyList<T> current, CancellationToken cancellation, [NotNull] T value) where T : IDependency {
            return current.AddAsync(asyncAnrMS, cancellation, value);
        }
        
        public static async Task AddAsync<T>(this ObservedDependencyList<T> current, int anr, CancellationToken cancellation, [NotNull] T value)
            where T : IDependency {
            if (_lock.TryAdd(current.id, true) == false) {
            #if UNITY_EDITOR || PERFORMANCE_DEBUG
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }
            
            current.list.Add(value);
            DateTime now = DateTime.Now;
            
            for (int i = current.onAdd.Count - 1; i >= 0; i--) {
                current.onAdd[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            for (int i = current.onAddWithValue.Count - 1; i >= 0; i--) {
                current.onAddWithValue[i].Invoke(value);
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
        
        public static Task RemoveAsync<T>(this ObservedDependencyList<T> current, [NotNull] params T[] values) where T : IDependency {
            return current.RemoveAsync(asyncAnrMS, AsyncUtility.token, values);
        }
        
        public static Task RemoveAsync<T>(this ObservedDependencyList<T> current, CancellationToken cancellation, [NotNull] params T[] values)
            where T : IDependency {
            return current.RemoveAsync(asyncAnrMS, cancellation, values);
        }
        
        public static async Task RemoveAsync<T>(this ObservedDependencyList<T> current, int anr, CancellationToken cancellation, [NotNull] params T[] values)
            where T : IDependency {
            if (_lock.TryAdd(current.id, true) == false) {
            #if UNITY_EDITOR || PERFORMANCE_DEBUG
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }
            
            for (int i = values.Length - 1; i >= 0; i--) {
                current.list.Remove(values[i]);
            }
            
            DateTime now = DateTime.Now;
            
            for (int i = current.onRemove.Count - 1; i >= 0; i--) {
                current.onRemove[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            for (int i = current.onRemoveWithValue.Count - 1; i >= 0; i--) {
                for (int j = 0; j < values.Length; j++) {
                    current.onRemoveWithValue[i].Invoke(values[j]);
                }
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
        
        public static Task RemoveAsync<T>(this ObservedDependencyList<T> current, [NotNull] T value) where T : IDependency {
            return current.RemoveAsync(asyncAnrMS, AsyncUtility.token, value);
        }
        
        public static Task RemoveAsync<T>(this ObservedDependencyList<T> current, CancellationToken cancellation, [NotNull] T value) where T : IDependency {
            return current.RemoveAsync(asyncAnrMS, cancellation, value);
        }
        
        public static async Task RemoveAsync<T>(this ObservedDependencyList<T> current, int anr, CancellationToken cancellation, [NotNull] T value)
            where T : IDependency {
            if (_lock.TryAdd(current.id, true) == false) {
            #if UNITY_EDITOR || PERFORMANCE_DEBUG
                Debug.LogError("ObservedList is locked!");
            #endif
                return;
            }
            
            current.list.Remove(value);
            DateTime now = DateTime.Now;
            
            for (int i = current.onRemove.Count - 1; i >= 0; i--) {
                current.onRemove[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            for (int i = current.onRemoveWithValue.Count - 1; i >= 0; i--) {
                current.onRemoveWithValue[i].Invoke(value);
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
        
        [Obsolete("Can`t use without parameters!", true)]
        public static Task AddAsync<T>(this ObservedDependencyList<T> current) where T : IDependency => default;
        
        [Obsolete("Can`t use without parameters!", true)]
        public static Task AddAsync<T>(this ObservedDependencyList<T> current, CancellationToken cancellation) where T : IDependency => default;
        
        [Obsolete("Can`t use without parameters!", true)]
        public static Task AddAsync<T>(this ObservedDependencyList<T> current, int anr, CancellationToken cancellation) where T : IDependency => default;
        
        [Obsolete("Can`t use without parameters!", true)]
        public static Task RemoveAsync<T>(this ObservedDependencyList<T> current) where T : IDependency => default;
        
        [Obsolete("Can`t use without parameters!", true)]
        public static Task RemoveAsync<T>(this ObservedDependencyList<T> current, CancellationToken cancellation) where T : IDependency => default;
        
        [Obsolete("Can`t use without parameters!", true)]
        public static Task RemoveAsync<T>(this ObservedDependencyList<T> current, int anr, CancellationToken cancellation) where T : IDependency => default;
    }
}