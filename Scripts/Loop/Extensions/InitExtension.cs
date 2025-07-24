// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TinyMVC.Loop.Extensions {
    public static class InitExtension {
        public static void TryInit<T>(this List<T> collection) {
            for (int i = 0; i < collection.Count; i++) {
                collection[i].TryInitSingle();
            }
        }
        
        public static void TryInit<T>(this T[] collection) {
            for (int i = 0; i < collection.Length; i++) {
                collection[i].TryInitSingle();
            }
        }
        
        public static async UniTask TryInitAsync<T>(this List<T> collection) {
            for (int i = 0; i < collection.Count; i++) {
                await collection[i].TryInitAsyncSingle();
            }
        }
        
        public static async UniTask TryInitAsync<T>(this T[] collection) {
            for (int i = 0; i < collection.Length; i++) {
                await collection[i].TryInitAsyncSingle();
            }
        }
        
        public static void Init<T>(this List<T> collection) where T : IInit {
            for (int i = 0; i < collection.Count; i++) {
                try {
                    collection[i].Init();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static void Init<T>(this T[] collection) where T : IInit {
            for (int i = 0; i < collection.Length; i++) {
                try {
                    collection[i].Init();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static async UniTask InitAsync<T>(this List<T> collection) where T : IInitAsync {
            for (int i = 0; i < collection.Count; i++) {
                try {
                    await collection[i].Init();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static async UniTask InitAsync<T>(this T[] collection) where T : IInitAsync {
            for (int i = 0; i < collection.Length; i++) {
                try {
                    await collection[i].Init();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
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
        
        public static async UniTask TryInitAsyncSingle<T>(this T obj) {
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