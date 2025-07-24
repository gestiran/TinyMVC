// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ActionListenerExtension {
        public static void Invoke(this List<ActionListener> actions) {
            if (actions.Count <= 0) {
                return;
            }
            
            ActionListener[] temp = new ActionListener[actions.Count];
            actions.CopyTo(temp);
            
            foreach (ActionListener listener in temp) {
                try {
                    listener();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static void Invoke<T>(this List<ActionListener<T>> actions, T[] value) {
            if (actions.Count <= 0) {
                return;
            }
            
            ActionListener<T>[] temp = new ActionListener<T>[actions.Count];
            actions.CopyTo(temp);
            
            foreach (ActionListener<T> listener in temp) {
                foreach (T current in value) {
                    try {
                        listener(current);
                    } catch (Exception exception) {
                        Debug.LogException(exception);
                    }
                }
            }
        }
        
        public static void Invoke<T>(this List<ActionListener<T>> actions, T value) {
            if (actions.Count <= 0) {
                return;
            }
            
            ActionListener<T>[] temp = new ActionListener<T>[actions.Count];
            actions.CopyTo(temp);
            
            foreach (ActionListener<T> listener in temp) {
                try {
                    listener(value);   
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static void Invoke<T1, T2>(this List<ActionListener<T1, T2>> actions, T1 value1, T2 value2) {
            if (actions.Count <= 0) {
                return;
            }
            
            ActionListener<T1, T2>[] temp = new ActionListener<T1, T2>[actions.Count];
            actions.CopyTo(temp);
            
            foreach (ActionListener<T1, T2> listener in temp) {
                try {
                    listener(value1, value2);
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static void Invoke<T1, T2, T3>(this List<ActionListener<T1, T2, T3>> actions, T1 value1, T2 value2, T3 value3) {
            if (actions.Count <= 0) {
                return;
            }
            
            ActionListener<T1, T2, T3>[] temp = new ActionListener<T1, T2, T3>[actions.Count];
            actions.CopyTo(temp);
            
            foreach (ActionListener<T1, T2, T3> listener in temp) {
                try {
                    listener(value1, value2, value3);
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
    }
}