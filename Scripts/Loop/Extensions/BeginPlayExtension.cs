using System.Collections.Generic;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using TinyMVC.Exceptions;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class BeginPlayExtension {
        public static void TryBeginPlay<T>(this T[] objects) {
            for (int objId = 0; objId < objects.Length; objId++) {
                if (objects[objId] is IBeginPlay other) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    try {
                    #endif
                        
                        other.BeginPlay();
                        
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    } catch (Exception exception) {
                        throw new BeginPlayException(other, exception);
                    }
                #endif
                }
            }
        }
        
        public static void TryBeginPlay<T>(this List<T> objects) {
            for (int objId = 0; objId < objects.Count; objId++) {
                if (objects[objId] is IBeginPlay other) {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    try {
                    #endif
                        
                        other.BeginPlay();
                        
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    } catch (Exception exception) {
                        throw new BeginPlayException(other, exception);
                    }
                #endif
                }
            }
        }
        
        public static void BeginPlay<TBeginPlay>(this TBeginPlay[] objects) where TBeginPlay : IBeginPlay {
            for (int objId = 0; objId < objects.Length; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif
                        
                    objects[objId].BeginPlay();
                        
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new BeginPlayException(objects[objId], exception);
                }
            #endif
            }
        }
        
        public static void BeginPlay<TBeginPlay>(this List<TBeginPlay> objects) where TBeginPlay : IBeginPlay {
            for (int objId = 0; objId < objects.Count; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif
                        
                    objects[objId].BeginPlay();
                        
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new BeginPlayException(objects[objId], exception);
                }
            #endif
            }
        }
    }
}