using System;
using System.Collections.Generic;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ListenerExtension {
        public static void RemoveListener<T>(this List<T> list, Action listener) where T : Listener {
            int hash = listener.GetHashCode();
            
            for (int i = 0; i < list.Count; i++) {
                if (list[i].hash != hash) {
                    continue;
                }
                
                list.RemoveAt(i);
                return;
            }
        }

        public static void RemoveListener<T>(this List<Listener<T>> list, Action listener) {
            int hash = listener.GetHashCode();
            
            for (int i = 0; i < list.Count; i++) {
                if (list[i].hash != hash) {
                    continue;
                }
                
                list.RemoveAt(i);
                return;
            }
        }

        public static void RemoveListener<T>(this List<Listener<T>> list, Action<T> listener) {
            int hash = listener.GetHashCode();
            
            for (int i = 0; i < list.Count; i++) {
                if (list[i].hash != hash) {
                    continue;
                }
                
                list.RemoveAt(i);
                return;
            }
        }
        
        public static void RemoveListener<T>(this List<Listener<T>> list, MultipleListener<T> listener) {
            int hash = listener.GetHashCode();
            
            for (int i = 0; i < list.Count; i++) {
                if (list[i].hash != hash) {
                    continue;
                }
                
                list.RemoveAt(i);
                return;
            }
        }
        
        public static void RemoveListener<T1, T2>(this List<Listener<T1, T2>> list, Action listener) {
            int hash = listener.GetHashCode();
            
            for (int i = 0; i < list.Count; i++) {
                if (list[i].hash != hash) {
                    continue;
                }
                
                list.RemoveAt(i);
                return;
            }
        }
        
        public static void RemoveListener<T1, T2>(this List<Listener<T1, T2>> list, Action<T1, T2> listener) {
            int hash = listener.GetHashCode();
            
            for (int i = 0; i < list.Count; i++) {
                if (list[i].hash != hash) {
                    continue;
                }
                
                list.RemoveAt(i);
                return;
            }
        }
        
        public static void RemoveListener<T1, T2, T3>(this List<Listener<T1, T2, T3>> list, Action listener) {
            int hash = listener.GetHashCode();
            
            for (int i = 0; i < list.Count; i++) {
                if (list[i].hash != hash) {
                    continue;
                }
                
                list.RemoveAt(i);
                return;
            }
        }
        
        public static void RemoveListener<T1, T2, T3>(this List<Listener<T1, T2, T3>> list, Action<T1, T2, T3> listener) {
            int hash = listener.GetHashCode();
            
            for (int i = 0; i < list.Count; i++) {
                if (list[i].hash != hash) {
                    continue;
                }
                
                list.RemoveAt(i);
                return;
            }
        }
    }
}