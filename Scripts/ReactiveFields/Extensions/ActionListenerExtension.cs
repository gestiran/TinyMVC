// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ActionListenerExtension {
        public static void Invoke(this List<ActionListener> actions) {
            int count = actions.Count;
            
            if (count <= 0) {
                return;
            }
            
            if (CacheArray<ActionListener>.TryGet(count, out CacheArray<ActionListener>.Data cache)) {
                actions.CopyTo(cache.array);
                Invoke(cache.array, count);
                cache.isBusy = false;
            } else {
                ActionListener[] temp = new ActionListener[count];
                actions.CopyTo(temp);
                Invoke(temp, count);
            }
        }
        
        public static void Invoke<T>(this List<ActionListener<T>> actions, T value) {
            int count = actions.Count;
            
            if (count <= 0) {
                return;
            }
            
            if (CacheArray<ActionListener<T>>.TryGet(count, out CacheArray<ActionListener<T>>.Data cache)) {
                actions.CopyTo(cache.array);
                Invoke(cache.array, value, count);
                cache.isBusy = false;
            } else {
                ActionListener<T>[] temp = new ActionListener<T>[count];
                actions.CopyTo(temp);
                Invoke(temp, value, count);
            }
        }
        
        public static void Invoke<T>(this List<ActionListener<T>> actions, T[] value) {
            int count = actions.Count;
            
            if (count <= 0) {
                return;
            }
            
            if (CacheArray<ActionListener<T>>.TryGet(count, out CacheArray<ActionListener<T>>.Data cache)) {
                actions.CopyTo(cache.array);
                Invoke(cache.array, value, count);
                cache.isBusy = false;
            } else {
                ActionListener<T>[] temp = new ActionListener<T>[count];
                actions.CopyTo(temp);
                Invoke(temp, value, count);
            }
        }
        
        public static void Invoke<T1, T2>(this List<ActionListener<T1, T2>> actions, T1 value1, T2 value2) {
            int count = actions.Count;
            
            if (count <= 0) {
                return;
            }
            
            if (CacheArray<ActionListener<T1, T2>>.TryGet(count, out CacheArray<ActionListener<T1, T2>>.Data cache)) {
                actions.CopyTo(cache.array);
                Invoke(cache.array, value1, value2, count);
                cache.isBusy = false;
            } else {
                ActionListener<T1, T2>[] temp = new ActionListener<T1, T2>[count];
                actions.CopyTo(temp);
                Invoke(temp, value1, value2, count);
            }
        }
        
        public static void Invoke<T1, T2, T3>(this List<ActionListener<T1, T2, T3>> actions, T1 value1, T2 value2, T3 value3) {
            int count = actions.Count;
            
            if (count <= 0) {
                return;
            }
            
            if (CacheArray<ActionListener<T1, T2, T3>>.TryGet(count, out CacheArray<ActionListener<T1, T2, T3>>.Data cache)) {
                actions.CopyTo(cache.array);
                Invoke(cache.array, value1, value2, value3, count);
                cache.isBusy = false;
            } else {
                ActionListener<T1, T2, T3>[] temp = new ActionListener<T1, T2, T3>[count];
                actions.CopyTo(temp);
                Invoke(temp, value1, value2, value3, count);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Invoke(ActionListener[] actions, int size) {
            for (int i = 0; i < size; i++) {
                try {
                    actions[i].Invoke();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Invoke<T>(ActionListener<T>[] actions, T value, int size) {
            for (int i = 0; i < size; i++) {
                try {
                    actions[i].Invoke(value);
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Invoke<T>(ActionListener<T>[] actions, T[] value, int size) {
            for (int i = 0; i < size; i++) {
                foreach (T current in value) {
                    try {
                        actions[i].Invoke(current);
                    } catch (Exception exception) {
                        Debug.LogException(exception);
                    }
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Invoke<T1, T2>(ActionListener<T1, T2>[] actions, T1 value1, T2 value2, int size) {
            for (int i = 0; i < size; i++) {
                try {
                    actions[i].Invoke(value1, value2);
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Invoke<T1, T2, T3>(ActionListener<T1, T2, T3>[] actions, T1 value1, T2 value2, T3 value3, int size) {
            for (int i = 0; i < size; i++) {
                try {
                    actions[i].Invoke(value1, value2, value3);
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
    }
}