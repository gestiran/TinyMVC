﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TinyMVC.Loop.Extensions {
    public static class UnloadExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryUnload<T>(this ICollection<T> collection) {
            foreach (T obj in collection) {
                obj.TryUnloadSingle();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unload<T>(this ICollection<T> collection) where T : IUnload {
            foreach (T obj in collection) {
                try {
                    obj.Unload();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unload<TUnload>(this Dictionary<TUnload, TUnload> objects) where TUnload : IUnload {
            foreach (KeyValuePair<TUnload, TUnload> unload in objects) {
                try {
                    unload.Key.Unload();
                    unload.Value.Unload();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryUnloadKeys<TUnload, T>(this Dictionary<TUnload, T> objects) {
            foreach (TUnload unload in objects.Keys) {
                unload.TryUnloadSingle();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadKeys<TUnload, T>(this Dictionary<TUnload, T> objects) where TUnload : IUnload {
            foreach (TUnload unload in objects.Keys) {
                try {
                    unload.Unload();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryUnloadValues<T1, T2>(this Dictionary<T1, T2> objects) {
            foreach (T2 unload in objects.Values) {
                unload.TryUnloadSingle();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadValues<T, TUnload>(this Dictionary<T, TUnload> objects) where TUnload : IUnload {
            foreach (TUnload unload in objects.Values) {
                unload.TryUnloadSingle();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryUnloadSingle<T>(this T obj) {
            if (obj is IUnload other) {
                try {
                    other.Unload();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
    }
}