using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyMVC.Debugging;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ActionExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Invoke(this List<Action> actions) {
            if (actions.Count <= 0) {
                return;
            }
            
            Action[] temp = new Action[actions.Count];
            int i;
            
            for (i = 0; i < actions.Count; i++) {
                temp[i] = actions[i];
            }
            
            for (i = 0; i < temp.Length; i++) {
                DebugUtility.CheckAndLogException(temp[i].Invoke);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Invoke<T>(this List<Action<T>> actions, T value) {
            if (actions.Count <= 0) {
                return;
            }
            
            Action<T>[] temp = new Action<T>[actions.Count];
            int i;
            
            for (i = 0; i < actions.Count; i++) {
                temp[i] = actions[i];
            }
            
            for (i = 0; i < temp.Length; i++) {
                DebugUtility.CheckAndLogException(temp[i].Invoke, value);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Invoke<T>(this List<Action<T>> actions, T[] value) {
            if (actions.Count <= 0) {
                return;
            }
            
            Action<T>[] temp = new Action<T>[actions.Count];
            int i;
            
            for (i = 0; i < actions.Count; i++) {
                temp[i] = actions[i];
            }
            
            for (i = 0; i < temp.Length; i++) {
                for (int v = 0; v < value.Length; v++) {
                    DebugUtility.CheckAndLogException(temp[i].Invoke, value[v]);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Invoke<T1, T2>(this List<Action<T1, T2>> actions, T1 value1, T2 value2) {
            if (actions.Count <= 0) {
                return;
            }
            
            Action<T1, T2>[] temp = new Action<T1, T2>[actions.Count];
            int i;
            
            for (i = 0; i < actions.Count; i++) {
                temp[i] = actions[i];
            }
            
            for (i = 0; i < temp.Length; i++) {
                DebugUtility.CheckAndLogException(temp[i].Invoke, value1, value2);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Invoke<T1, T2, T3>(this List<Action<T1, T2, T3>> actions, T1 value1, T2 value2, T3 value3) {
            if (actions.Count <= 0) {
                return;
            }
            
            Action<T1, T2, T3>[] temp = new Action<T1, T2, T3>[actions.Count];
            int i;
            
            for (i = 0; i < actions.Count; i++) {
                temp[i] = actions[i];
            }
            
            for (i = 0; i < temp.Length; i++) {
                DebugUtility.CheckAndLogException(temp[i].Invoke, value1, value2, value3);
            }
        }
    }
}