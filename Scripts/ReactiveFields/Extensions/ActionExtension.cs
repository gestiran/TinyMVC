// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ActionExtension {
        public static void Invoke(this List<Action> actions) {
            if (actions.Count <= 0) {
                return;
            }
            
            Action[] temp = new Action[actions.Count];
            actions.CopyTo(temp);
            
            foreach (Action listener in temp) {
                listener();
            }
        }
        
        public static void Invoke<T>(this List<Action<T>> actions, T[] value) {
            if (actions.Count <= 0) {
                return;
            }
            
            Action<T>[] temp = new Action<T>[actions.Count];
            actions.CopyTo(temp);
            
            foreach (Action<T> listener in temp) {
                foreach (T current in value) {
                    listener(current);
                }
            }
        }
        
        public static void Invoke<T>(this List<Action<T>> actions, T value) {
            if (actions.Count <= 0) {
                return;
            }
            
            Action<T>[] temp = new Action<T>[actions.Count];
            actions.CopyTo(temp);
            
            foreach (Action<T> listener in temp) {
                listener(value);
            }
        }
        
        public static void Invoke<T1, T2>(this List<Action<T1, T2>> actions, T1 value1, T2 value2) {
            if (actions.Count <= 0) {
                return;
            }
            
            Action<T1, T2>[] temp = new Action<T1, T2>[actions.Count];
            actions.CopyTo(temp);
            
            foreach (Action<T1, T2> listener in actions) {
                listener(value1, value2);
            }
        }
        
        public static void Invoke<T1, T2, T3>(this List<Action<T1, T2, T3>> actions, T1 value1, T2 value2, T3 value3) {
            if (actions.Count <= 0) {
                return;
            }
            
            Action<T1, T2, T3>[] temp = new Action<T1, T2, T3>[actions.Count];
            actions.CopyTo(temp);
            
            foreach (Action<T1, T2, T3> listener in temp) {
                listener(value1, value2, value3);
            }
        }
    }
}