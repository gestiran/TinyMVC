using System.Collections.Generic;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using TinyMVC.Exceptions;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class InitExtension {
        public static void TryInit<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IInit other) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    try {
                    #endif
                        
                        other.Init();
                        
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    } catch (Exception exception) {
                        throw new InitException(other, exception);
                    }
                #endif
                }
            }
        }

        public static void TryInit<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IInit other) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    try {
                    #endif
                        
                        other.Init();
                        
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    } catch (Exception exception) {
                        throw new InitException(other, exception);
                    }
                #endif
                }
            }
        }

        public static void Init<TInit>(this TInit[] objects) where TInit : IInit {
            for (int objId = 0; objId < objects.Length; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif
                    
                    objects[objId].Init();
                    
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new InitException(objects[objId], exception);
                }
            #endif
            }
        }

        public static void Init<TInit>(this List<TInit> objects) where TInit : IInit {
            for (int objId = 0; objId < objects.Count; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif
                    
                    objects[objId].Init();
                    
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new InitException(objects[objId], exception);
                }
            #endif
            }
        }
    }
}