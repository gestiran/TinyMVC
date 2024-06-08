using System.Collections.Generic;
using System.Threading.Tasks;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TinyMVC.Debugging;
using TinyMVC.Debugging.Exceptions;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class InitExtension {
        public static void TryInit<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IInitAsync otherAsync) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => otherAsync.Init(), exception => new InitAsyncException(otherAsync, exception));
                    #else
                    otherAsync.Init();
                    #endif
                } else if (objects[objId] is IInit other) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => other.Init(), exception => new InitException(other, exception));
                    #else
                    other.Init();
                    #endif
                }
            }
        }
        
        public static async Task TryInitAsync<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IInitAsync otherAsync) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    await DebugUtility.ReThrowAsync(() => otherAsync.Init(), exception => new InitAsyncException(otherAsync, exception));
                    #else
                    await otherAsync.Init();
                    #endif
                } else if (objects[objId] is IInit other) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => other.Init(), exception => new InitException(other, exception));
                    #else
                    other.Init();
                    #endif
                }
            }
        }
        
        public static void TryInit<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IInitAsync otherAsync) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => otherAsync.Init(), exception => new InitAsyncException(otherAsync, exception));
                    #else
                    otherAsync.Init();
                    #endif
                } else if (objects[objId] is IInit other) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => other.Init(), exception => new InitException(other, exception));
                    #else
                    other.Init();
                    #endif
                }
            }
        }
        
        public static async Task TryInitAsync<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IInitAsync otherAsync) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    await DebugUtility.ReThrowAsync(() => otherAsync.Init(), exception => new InitAsyncException(otherAsync, exception));
                    #else
                    await otherAsync.Init();
                    #endif
                } else if (objects[objId] is IInit other) {
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugUtility.ReThrow(() => other.Init(), exception => new InitException(other, exception));
                    #else
                    other.Init();
                    #endif
                }
            }
        }
        
        public static void Init<TInit>(this TInit[] objects) where TInit : IInit {
            for (int objId = 0; objId < objects.Length; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].Init(), exception => new InitException(objects[id], exception));
                #else
                objects[objId].Init();
                #endif
            }
        }
        
        public static async Task InitAsync<TInit>(this TInit[] objects) where TInit : IInitAsync {
            for (int objId = 0; objId < objects.Length; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                await DebugUtility.ReThrowAsync(() => objects[id].Init(), exception => new InitAsyncException(objects[id], exception));
                #else
                await objects[objId].Init();
                #endif
            }
        }
        
        public static void Init<TInit>(this List<TInit> objects) where TInit : IInit {
            for (int objId = 0; objId < objects.Count; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                DebugUtility.ReThrow(() => objects[id].Init(), exception => new InitException(objects[id], exception));
                #else
                objects[objId].Init();
                #endif
            }
        }
        
        public static async Task InitAsync<TInit>(this List<TInit> objects) where TInit : IInitAsync {
            for (int objId = 0; objId < objects.Count; objId++) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                int id = objId;
                await DebugUtility.ReThrowAsync(() => objects[id].Init(), exception => new InitAsyncException(objects[id], exception));
                #else
                await objects[objId].Init();
                #endif
            }
        }
    }
}