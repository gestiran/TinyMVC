// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TinyMVC.Loop.Extensions {
    public static class BeginPlayExtension {
        public static void TryBeginPlay<T>(this List<T> collection) {
            for (int i = 0; i < collection.Count; i++) {
                collection[i].TryBeginPlaySingle();
            }
        }
        
        public static void TryBeginPlay<T>(this T[] collection) {
            for (int i = 0; i < collection.Length; i++) {
                collection[i].TryBeginPlaySingle();
            }
        }
        
        public static async UniTask TryBeginPlayAsync<T>(this List<T> collection) {
            for (int i = 0; i < collection.Count; i++) {
                await collection[i].TryBeginPlayAsyncSingle();
            }
        }
        
        public static async UniTask TryBeginPlayAsync<T>(this T[] collection) {
            for (int i = 0; i < collection.Length; i++) {
                await collection[i].TryBeginPlayAsyncSingle();
            }
        }
        
        public static void BeginPlay<T>(this List<T> collection) where T : IBeginPlay {
            for (int i = 0; i < collection.Count; i++) {
                try {
                    collection[i].BeginPlay();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static void BeginPlay<T>(this T[] collection) where T : IBeginPlay {
            for (int i = 0; i < collection.Length; i++) {
                try {
                    collection[i].BeginPlay();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static async UniTask BeginPlayAsync<T>(this List<T> collection) where T : IBeginPlayAsync {
            for (int i = 0; i < collection.Count; i++) {
                try {
                    await collection[i].BeginPlay();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static async UniTask BeginPlayAsync<T>(this T[] collection) where T : IBeginPlayAsync {
            for (int i = 0; i < collection.Length; i++) {
                try {
                    await collection[i].BeginPlay();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
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