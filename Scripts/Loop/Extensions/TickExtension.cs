using System.Collections.Generic;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using TinyMVC.Exceptions;
#endif

namespace TinyMVC.Loop.Extensions {
    public static class TickExtension {
        public static void Tick<T>(this T[] objects) where T : ITick {
            for (int objId = 0; objId < objects.Length; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif
                        
                    objects[objId].Tick();
                        
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new TickException(objects[objId], exception);
                }
            #endif
            }
        }

        public static void Tick<T>(this List<T> objects) where T : ITick {
            for (int objId = 0; objId < objects.Count; objId++) {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
                try {
                #endif
                        
                    objects[objId].Tick();
                        
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                } catch (Exception exception) {
                    throw new TickException(objects[objId], exception);
                }
            #endif
            }
        }
    }
}