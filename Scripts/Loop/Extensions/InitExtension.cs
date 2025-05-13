using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TinyMVC.Loop.Extensions {
    public static class InitExtension {
        public static void TryInit<T>(this ICollection<T> collection) {
            foreach (T obj in collection) {
                obj.TryInitSingle();
            }
        }
        
        public static async UniTask TryInitAsync<T>(this ICollection<T> collection) {
            foreach (T obj in collection) {
                await obj.TryInitAsyncSingle();
            }
        }
        
        public static void Init<T>(this ICollection<T> collection) where T : IInit {
            foreach (T obj in collection) {
                try {
                    obj.Init();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static async UniTask InitAsync<T>(this ICollection<T> collection) where T : IInitAsync {
            foreach (T obj in collection) {
                try {
                    await obj.Init();
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