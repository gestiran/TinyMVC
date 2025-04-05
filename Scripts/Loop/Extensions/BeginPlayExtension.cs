using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TinyMVC.Loop.Extensions {
    public static class BeginPlayExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryBeginPlay<T>(this ICollection<T> collection) {
            foreach (T obj in collection) {
                obj.TryBeginPlaySingle();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async UniTask TryBeginPlayAsync<T>(this ICollection<T> collection) {
            foreach (T obj in collection) {
                await obj.TryBeginPlayAsyncSingle();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BeginPlay<T>(this ICollection<T> collection) where T : IBeginPlay {
            foreach (T obj in collection) {
                try {
                    obj.BeginPlay();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async UniTask BeginPlayAsync<T>(this ICollection<T> collection) where T : IBeginPlayAsync {
            foreach (T obj in collection) {
                try {
                    await obj.BeginPlay();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryBeginPlaySingle<T>(this T obj) {
            if (obj is IBeginPlayAsync otherAsync) {
                try {
                    otherAsync.BeginPlay();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            } else if (obj is IBeginPlay other) {
                try {
                    other.BeginPlay();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async UniTask TryBeginPlayAsyncSingle<T>(this T obj)  {
            if (obj is IBeginPlayAsync otherAsync) {
                try {
                    await otherAsync.BeginPlay();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            } else if (obj is IBeginPlay other) {
                try {
                    other.BeginPlay();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
    }
}