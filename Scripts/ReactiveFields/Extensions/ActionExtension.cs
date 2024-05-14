using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ActionExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Invoke(this List<Action> actions) {
            if (actions.Count > 0) {
                Action[] temp = new Action[actions.Count];
                int i = 0;
            
                for (i = 0; i < actions.Count; i++) {
                    temp[i] = actions[i];
                }

                for (i = 0; i < temp.Length; i++) {
                    temp[i].Invoke();
                }
            }
        } 
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Invoke<T>(this List<Action<T>> actions, T value) {
            if (actions.Count > 0) {
                Action<T>[] temp = new Action<T>[actions.Count];
                int i = 0;

                for (i = 0; i < actions.Count; i++) {
                    temp[i] = actions[i];
                }

                for (i = 0; i < temp.Length; i++) {
                    temp[i].Invoke(value);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Invoke<T>(this List<Action<T>> actions, T[] value) {
            if (actions.Count > 0) {
                Action<T>[] temp = new Action<T>[actions.Count];
                int i = 0;

                for (i = 0; i < actions.Count; i++) {
                    temp[i] = actions[i];
                }

                for (i = 0; i < temp.Length; i++) {
                    for (int v = 0; v < value.Length; v++) {
                        temp[i].Invoke(value[i]);   
                    }
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Invoke<T1, T2>(this List<Action<T1, T2>> actions, T1 value1, T2 value2) {
            if (actions.Count > 0) {
                Action<T1, T2>[] temp = new Action<T1, T2>[actions.Count];
                int i = 0;

                for (i = 0; i < actions.Count; i++) {
                    temp[i] = actions[i];
                }

                for (i = 0; i < temp.Length; i++) {
                    temp[i].Invoke(value1, value2);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Invoke<T1, T2, T3>(this List<Action<T1, T2, T3>> actions, T1 value1, T2 value2, T3 value3) {
            if (actions.Count > 0) {
                Action<T1, T2, T3>[] temp = new Action<T1, T2, T3>[actions.Count];
                int i = 0;

                for (i = 0; i < actions.Count; i++) {
                    temp[i] = actions[i];
                }

                for (i = 0; i < temp.Length; i++) {
                    temp[i].Invoke(value1, value2, value3);
                }
            }
        }
    }
}