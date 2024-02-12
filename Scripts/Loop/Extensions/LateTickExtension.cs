using System.Collections.Generic;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using TinyMVC.Exceptions;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class LateTickExtension {
        public static void LateTick<T>(this T[] objects) where T : ILateTick {
            for (int objId = 0; objId < objects.Length; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif
                        
                    objects[objId].LateTick();
                        
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new LateTickException(objects[objId], exception);
                }
            #endif
            }
        }

        public static void LateTick<T>(this List<T> objects) where T : ILateTick {
            for (int objId = 0; objId < objects.Count; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif
                        
                    objects[objId].LateTick();
                        
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new LateTickException(objects[objId], exception);
                }
            #endif
            }
        }
    }
}