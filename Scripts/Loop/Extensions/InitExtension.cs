using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace TinyMVC.Loop.Extensions {
    public static class InitExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryInit<T>(this ICollection<T> collection) {
            foreach (T obj in collection) {
                obj.TryInitSingle();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task TryInitAsync<T>(this ICollection<T> collection) {
            foreach (T obj in collection) {
                await obj.TryInitAsyncSingle();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init<T>(this ICollection<T> collection) where T : IInit {
            foreach (T obj in collection) {
                try {
                    obj.Init();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task InitAsync<T>(this ICollection<T> collection) where T : IInitAsync {
            foreach (T obj in collection) {
                try {
                    await obj.Init();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryInitSingle<T>(this T obj) {
            if (obj is IInitAsync otherAsync) {
                try {
                    otherAsync.Init();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            } else if (obj is IInit other) {
                try {
                    other.Init();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task TryInitAsyncSingle<T>(this T obj) {
            if (obj is IInitAsync otherAsync) {
                try {
                    await otherAsync.Init();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            } else if (obj is IInit other) {
                try {
                    other.Init();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
    }
}