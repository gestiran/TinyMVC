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
            
            for (int i = 0; i < temp.Length; i++) {
                try {
                    temp[i].Invoke();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static void Invoke(this Dictionary<int, ActionListener> actions) {
            int count = actions.Count;
            
            if (count <= 0) {
                return;
            }
            
            ActionListener[] temp = new ActionListener[count];
            count = 0;
            
            foreach (ActionListener action in actions.Values) {
                temp[count++] = action;
            }
            
            for (count = 0; count < temp.Length; count++) {
                try {
                    temp[count].Invoke();
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static void Invoke<T>(this List<ActionListener<T>> actions, T[] values) {
            if (actions.Count <= 0) {
                return;
            }
            
            ActionListener<T>[] temp = new ActionListener<T>[actions.Count];
            actions.CopyTo(temp);
            
            for (int i = 0; i < temp.Length; i++) {
                ActionListener<T> listener = temp[i];
                
                for (int j = 0; j < values.Length; j++) {
                    try {
                        listener.Invoke(values[j]);
                    } catch (Exception exception) {
                        Debug.LogException(exception);
                    }
                }
            }
        }
        
        public static void Invoke<T>(this Dictionary<int, ActionListener<T>> actions, T[] values) {
            int count = actions.Count;
            
            if (count <= 0) {
                return;
            }
            
            ActionListener<T>[] temp = new ActionListener<T>[count];
            count = 0;
            
            foreach (ActionListener<T> action in actions.Values) {
                temp[count++] = action;
            }
            
            for (count = 0; count < temp.Length; count++) {
                for (int i = 0; i < values.Length; i++) {
                    try {
                        temp[count].Invoke(values[i]);
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
            
            for (int i = 0; i < temp.Length; i++) {
                try {
                    temp[i].Invoke(value);
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static void Invoke<T>(this Dictionary<int, ActionListener<T>> actions, T value) {
            int count = actions.Count;
            
            if (count <= 0) {
                return;
            }
            
            ActionListener<T>[] temp = new ActionListener<T>[count];
            count = 0;
            
            foreach (ActionListener<T> action in actions.Values) {
                temp[count++] = action;
            }
            
            for (count = 0; count < temp.Length; count++) {
                try {
                    temp[count].Invoke(value);
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
            
            for (int i = 0; i < temp.Length; i++) {
                try {
                    temp[i].Invoke(value1, value2);
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static void Invoke<T1, T2>(this Dictionary<int, ActionListener<T1, T2>> actions, T1 value1, T2 value2) {
            int count = actions.Count;
            
            if (count <= 0) {
                return;
            }
            
            ActionListener<T1, T2>[] temp = new ActionListener<T1, T2>[count];
            count = 0;
            
            foreach (ActionListener<T1, T2> action in actions.Values) {
                temp[count++] = action;
            }
            
            for (count = 0; count < temp.Length; count++) {
                try {
                    temp[count].Invoke(value1, value2);
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
            
            for (int i = 0; i < temp.Length; i++) {
                try {
                    temp[i].Invoke(value1, value2, value3);
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
        
        public static void Invoke<T1, T2, T3>(this Dictionary<int, ActionListener<T1, T2, T3>> actions, T1 value1, T2 value2, T3 value3) {
            int count = actions.Count;
            
            if (count <= 0) {
                return;
            }
            
            ActionListener<T1, T2, T3>[] temp = new ActionListener<T1, T2, T3>[count];
            count = 0;
            
            foreach (ActionListener<T1, T2, T3> action in actions.Values) {
                temp[count++] = action;
            }
            
            for (count = 0; count < temp.Length; count++) {
                try {
                    temp[count].Invoke(value1, value2, value3);
                } catch (Exception exception) {
                    Debug.LogException(exception);
                }
            }
        }
    }
}