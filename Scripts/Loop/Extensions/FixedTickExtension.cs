using System.Collections.Generic;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using TinyMVC.Exceptions;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class FixedTickExtension {
        public static void FixedTick<T>(this T[] objects) where T : IFixedTick {
            for (int objId = 0; objId < objects.Length; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif
                        
                    objects[objId].FixedTick();
                        
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new FixedTickException(objects[objId], exception);
                }
            #endif
            }
        }

        public static void FixedTick<T>(this List<T> objects) where T : IFixedTick {
            for (int objId = 0; objId < objects.Count; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif
                        
                    objects[objId].FixedTick();
                        
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new FixedTickException(objects[objId], exception);
                }
            #endif
            }
        }
    }
}