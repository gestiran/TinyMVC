using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TinyMVC.Loop;

namespace TinyMVC.ReactiveFields {
    public static class InputListenerExtension {
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener(this InputListener obj, ActionListener listener) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list)) {
                list.Add(listener);
            } else {
                Listeners.pool.Add(obj.id, new List<ActionListener>() { listener });
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener(this InputListener obj, ActionListener listener, UnloadPool unload) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list)) {
                list.Add(listener);
            } else {
                list = new List<ActionListener>() { listener };
                Listeners.pool.Add(obj.id, list);
            }
            
            unload.Add(new UnloadAction(() => list.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T>(this InputListener<T> obj, ActionListener listener) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list)) {
                list.Add(listener);
            } else {
                Listeners.pool.Add(obj.id, new List<ActionListener>() { listener });
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T>(this InputListener<T> obj, ActionListener listener, UnloadPool unload) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list)) {
                list.Add(listener);
            } else {
                list = new List<ActionListener>() { listener };
                Listeners.pool.Add(obj.id, list);
            }
            
            unload.Add(new UnloadAction(() => list.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T>(this InputListener<T> obj, ActionListener<T> listener) {
            if (Listeners<T>.pool.TryGetValue(obj.id, out List<ActionListener<T>> list)) {
                list.Add(listener);
            } else {
                Listeners<T>.pool.Add(obj.id, new List<ActionListener<T>>() { listener });
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T>(this InputListener<T> obj, ActionListener<T> listener, UnloadPool unload) {
            if (Listeners<T>.pool.TryGetValue(obj.id, out List<ActionListener<T>> list)) {
                list.Add(listener);
            } else {
                list = new List<ActionListener<T>>() { listener };
                Listeners<T>.pool.Add(obj.id, list);
            }
            
            unload.Add(new UnloadAction(() => list.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T1, T2>(this InputListener<T1, T2> obj, ActionListener listener) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list)) {
                list.Add(listener);
            } else {
                Listeners.pool.Add(obj.id, new List<ActionListener>() { listener });
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T1, T2>(this InputListener<T1, T2> obj, ActionListener listener, UnloadPool unload) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list)) {
                list.Add(listener);
            } else {
                list = new List<ActionListener>() { listener };
                Listeners.pool.Add(obj.id, list);
            }
            
            unload.Add(new UnloadAction(() => list.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T1, T2>(this InputListener<T1, T2> obj, ActionListener<T1, T2> listener) {
            if (Listeners<T1, T2>.pool.TryGetValue(obj.id, out List<ActionListener<T1, T2>> list)) {
                list.Add(listener);
            } else {
                Listeners<T1, T2>.pool.Add(obj.id, new List<ActionListener<T1, T2>>() { listener });
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T1, T2>(this InputListener<T1, T2> obj, ActionListener<T1, T2> listener, UnloadPool unload) {
            if (Listeners<T1, T2>.pool.TryGetValue(obj.id, out List<ActionListener<T1, T2>> list)) {
                list.Add(listener);
            } else {
                list = new List<ActionListener<T1, T2>>() { listener };
                Listeners<T1, T2>.pool.Add(obj.id, list);
            }
            
            unload.Add(new UnloadAction(() => list.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T1, T2, T3>(this InputListener<T1, T2, T3> obj, ActionListener listener) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list)) {
                list.Add(listener);
            } else {
                Listeners.pool.Add(obj.id, new List<ActionListener>() { listener });
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T1, T2, T3>(this InputListener<T1, T2, T3> obj, ActionListener listener, UnloadPool unload) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list)) {
                list.Add(listener);
            } else {
                list = new List<ActionListener>() { listener };
                Listeners.pool.Add(obj.id, list);
            }
            
            unload.Add(new UnloadAction(() => list.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T1, T2, T3>(this InputListener<T1, T2, T3> obj, ActionListener<T1, T2, T3> listener) {
            if (Listeners<T1, T2, T3>.pool.TryGetValue(obj.id, out List<ActionListener<T1, T2, T3>> list)) {
                list.Add(listener);
            } else {
                Listeners<T1, T2, T3>.pool.Add(obj.id, new List<ActionListener<T1, T2, T3>>() { listener });
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T1, T2, T3>(this InputListener<T1, T2, T3> obj, ActionListener<T1, T2, T3> listener, UnloadPool unload) {
            if (Listeners<T1, T2, T3>.pool.TryGetValue(obj.id, out List<ActionListener<T1, T2, T3>> list)) {
                list.Add(listener);
            } else {
                list = new List<ActionListener<T1, T2, T3>>() { listener };
                Listeners<T1, T2, T3>.pool.Add(obj.id, list);
            }
            
            unload.Add(new UnloadAction(() => list.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListener(this InputListener obj, ActionListener listener) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list) == false) {
                return;
            }
            
            list.Remove(listener);
            
            if (list.Count > 0) {
                return;
            }
            
            Listeners.pool.Remove(obj.id);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListener<T>(this InputListener<T> obj, ActionListener listener) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list) == false) {
                return;
            }
            
            list.Remove(listener);
            
            if (list.Count > 0) {
                return;
            }
            
            Listeners.pool.Remove(obj.id);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListener<T>(this InputListener<T> obj, ActionListener<T> listener) {
            if (Listeners<T>.pool.TryGetValue(obj.id, out List<ActionListener<T>> list) == false) {
                return;
            }
            
            list.Remove(listener);
            
            if (list.Count > 0) {
                return;
            }
            
            Listeners<T>.pool.Remove(obj.id);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListener<T1, T2>(this InputListener<T1, T2> obj, ActionListener listener) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list) == false) {
                return;
            }
            
            list.Remove(listener);
            
            if (list.Count > 0) {
                return;
            }
            
            Listeners.pool.Remove(obj.id);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListener<T1, T2>(this InputListener<T1, T2> obj, ActionListener<T1, T2> listener) {
            if (Listeners<T1, T2>.pool.TryGetValue(obj.id, out List<ActionListener<T1, T2>> list) == false) {
                return;
            }
            
            list.Remove(listener);
            
            if (list.Count > 0) {
                return;
            }
            
            Listeners<T1, T2>.pool.Remove(obj.id);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListener<T1, T2, T3>(this InputListener<T1, T2, T3> obj, ActionListener listener) {
            if (Listeners.pool.TryGetValue(obj.id, out List<ActionListener> list) == false) {
                return;
            }
            
            list.Remove(listener);
            
            if (list.Count > 0) {
                return;
            }
            
            Listeners.pool.Remove(obj.id);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListener<T1, T2, T3>(this InputListener<T1, T2, T3> obj, ActionListener<T1, T2, T3> listener) {
            if (Listeners<T1, T2, T3>.pool.TryGetValue(obj.id, out List<ActionListener<T1, T2, T3>> list) == false) {
                return;
            }
            
            list.Remove(listener);
            
            if (list.Count > 0) {
                return;
            }
            
            Listeners<T1, T2, T3>.pool.Remove(obj.id);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListeners(this InputListener obj) {
            Listeners.pool.Remove(obj.id);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListeners<T>(this InputListener<T> obj) {
            Listeners<T>.pool.Remove(obj.id);
            Listeners.pool.Remove(obj.id);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListeners<T1, T2>(this InputListener<T1, T2> obj) {
            Listeners<T1, T2>.pool.Remove(obj.id);
            Listeners.pool.Remove(obj.id);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListeners<T1, T2, T3>(this InputListener<T1, T2, T3> obj) {
            Listeners<T1, T2, T3>.pool.Remove(obj.id);
            Listeners.pool.Remove(obj.id);
        }
        
    #endregion
    }
}