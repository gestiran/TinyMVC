using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TinyMVC.Boot;
using TinyMVC.Loop;
using UnityEngine;

namespace TinyMVC.ReactiveFields {
    public static class ObservedExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Increment(this Observed<int> observed) => observed.Set(observed.value + 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Increment(this Observed<float> observed) => observed.Set(observed.value + 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Decrement(this Observed<int> observed) => observed.Set(observed.value - 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Decrement(this Observed<float> observed) => observed.Set(observed.value - 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValue(this Observed<int> observed, int value) => observed.Set(observed.value + value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValueSilent(this Observed<int> observed, int value) => observed.SetSilent(observed.value + value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValueIfAvailable(this Observed<int> observed, int value) {
            if (value == 0) {
                return;
            }
            
            observed.AddValue(value);
        }
        
        [Obsolete("Can't add nothing!", true)]
        public static void AddValue(this Observed<int> observed) {
            // Do nothing
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCurrent(this Observed<int> observed, int current) => observed.value == current;
        
        [Obsolete("Can't add nothing!", true)]
        public static void AddValue(this Observed<float> observed) {
            // Do nothing
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValueMax(this Observed<int> observed, int value, int maxValue) => observed.Set(Mathf.Min(observed.value + value, maxValue));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValue(this Observed<int> observed, [NotNull] params int[] values) {
            int value = observed.value;
            
            for (int i = 0; i < values.Length; i++) {
                value += values[i];
            }
            
            observed.Set(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValue(this Observed<float> observed, float value) => observed.Set(observed.value + value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValueIfAvailable(this Observed<float> observed, float value) {
            if (value == 0) {
                return;
            }
            
            observed.AddValue(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValueMax(this Observed<float> observed, float value, float maxValue) => observed.Set(Mathf.Min(observed.value + value, maxValue));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValue(this Observed<float> observed, [NotNull] float[] values) {
            float value = observed.value;
            
            for (int i = 0; i < values.Length; i++) {
                value += values[i];
            }
            
            observed.Set(value);
        }
        
        [Obsolete("Can't subtract nothing!", true)]
        public static void SubtractValue(this Observed<int> observed) {
            // Do nothing
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValue(this Observed<int> observed, int value) => observed.Set(observed.value - value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueSilent(this Observed<int> observed, int value) => observed.SetSilent(observed.value - value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueIfAvailable(this Observed<int> observed, int value) {
            if (value == 0) {
                return;
            }
            
            observed.SubtractValue(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueIfAvailableLimit(this Observed<int> observed, int value, int limit = 0) {
            if (value == 0) {
                return;
            }
            
            observed.SubtractValueLimit(value, limit);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueLimit(this Observed<int> observed, int value, int limit = 0) {
            observed.Set(Mathf.Max(observed.value - value, limit));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValue(this Observed<int> observed, [NotNull] params int[] values) {
            int value = observed.value;
            
            for (int i = 0; i < values.Length; i++) {
                value -= values[i];
            }
            
            observed.Set(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValue(this Observed<float> observed, float value) => observed.Set(observed.value - value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueIfAvailable(this Observed<float> observed, float value) {
            if (value == 0) {
                return;
            }
            
            observed.SubtractValue(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueIfAvailableLimit(this Observed<float> observed, float value, float limit = 0) {
            if (value == 0) {
                return;
            }
            
            observed.SubtractValueLimit(value, limit);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueLimit(this Observed<float> observed, float value, float limit = 0) {
            observed.Set(Mathf.Max(observed.value - value, limit));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValue(this Observed<float> observed, [NotNull] params float[] values) {
            float value = observed.value;
            
            for (int i = 0; i < values.Length; i++) {
                value -= values[i];
            }
            
            observed.Set(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySet<T>(this Observed<T> observed, T value) {
            if (observed.value.Equals(value)) {
                return false;
            }
            
            observed.Set(value);
            
            return true;
        }
        
        public static void SubtractSecond(this Observed<TimeSpan> observed, int seconds) {
            observed.Set(observed.value.Subtract(new TimeSpan(0, 0, seconds)));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetWhen<T>(this Observed<T> observed, T equals, T set) {
            if (observed.value.Equals(equals) == false) {
                return false;
            }
            
            observed.Set(set);
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetNext(this Observed<int> observed, int value) {
            if (observed.value >= value) {
                return false;
            }
            
            observed.Set(value);
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetNext(this Observed<float> observed, float value) {
            if (observed.value >= value) {
                return false;
            }
            
            observed.Set(value);
            
            return true;
        }
        
        [Obsolete("Can't use without parameters!", true)]
        public static void TrySetMin(this Observed<int> observed) { }
        
        [Obsolete("Can't use without parameters!", true)]
        public static void TrySetMin(this Observed<float> observed) { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySetMin(this Observed<float> observed, params float[] values) => observed.TrySet(Mathf.Min(values));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySetMin(this Observed<int> observed, params int[] values) => observed.TrySet(Mathf.Min(values));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySetClamp(this Observed<int> observed, int value, int min, int max) => observed.TrySet(Mathf.Clamp(value, min, max));
        
        [Obsolete("Can't use without parameters!", true)]
        public static void TrySetMax(this Observed<int> observed) { }
        
        [Obsolete("Can't use without parameters!", true)]
        public static void TrySetMax(this Observed<float> observed) { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySetMax(this Observed<float> observed, params float[] values) => observed.TrySet(Mathf.Max(values));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySetMax(this Observed<int> observed, params int[] values) => observed.TrySet(Mathf.Max(values));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetPrevious(this Observed<int> observed, int value) {
            if (observed.value <= value) {
                return false;
            }
            
            observed.Set(value);
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetPrevious(this Observed<float> observed, float value) {
            if (observed.value <= value) {
                return false;
            }
            
            observed.Set(value);
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Toggle(this Observed<bool> observed) {
            if (observed.value) {
                observed.Set(false);
            } else {
                observed.Set(true);
            }
        }
        
    #region Log
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogChanges<T>(this Observed<T> observed, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            observed.AddListener(value => Debug.Log($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogChanges<T>(this Observed<T> observed, string label, UnloadPool unload) {
            observed.AddListener(value => Debug.Log($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarningChanges<T>(this Observed<T> observed, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            observed.AddListener(value => Debug.LogWarning($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarningChanges<T>(this Observed<T> observed, string label, UnloadPool unload) {
            observed.AddListener(value => Debug.LogWarning($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogErrorChanges<T>(this Observed<T> observed) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            observed.AddListener(value => Debug.LogError($"Observed: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogErrorChanges<T>(this Observed<T> observed, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            observed.AddListener(value => Debug.LogError($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogErrorChanges<T>(this Observed<T> observed, string label, UnloadPool unload) {
            observed.AddListener(value => Debug.LogError($"{label}: {value}"), unload);
        }
        
    #endregion
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T>(this Observed<T> observed, Action listener) {
            Listeners.pool[observed.id].Add(listener);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T>(this Observed<T> observed, Action listener, UnloadPool unload) {
            List<Action> pool = Listeners.pool[observed.id];
            pool.Add(listener);
            unload.Add(new UnloadAction(() => pool.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T>(this Observed<T> observed, Action<T> listener) {
            Listeners<T>.pool[observed.id].Add(listener);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener<T>(this Observed<T> observed, Action<T> listener, UnloadPool unload) {
            List<Action<T>> pool = Listeners<T>.pool[observed.id];
            pool.Add(listener);
            unload.Add(new UnloadAction(() => pool.Remove(listener)));
        }
        
    #endregion
        
    #region ByPriority
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListenerFirst<T>(this Observed<T> observed, Action listener) {
            List<Action> pool = Listeners.pool[observed.id];
            
            if (pool.Count > 0) {
                pool.Insert(0, listener);
            } else {
                pool.Add(listener);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListenerFirst<T>(this Observed<T> observed, Action listener, UnloadPool unload) {
            List<Action> pool = Listeners.pool[observed.id];
            
            if (pool.Count > 0) {
                pool.Insert(0, listener);
            } else {
                pool.Add(listener);
            }
            
            unload.Add(new UnloadAction(() => pool.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListenerFirst<T>(this Observed<T> observed, Action<T> listener) {
            List<Action<T>> pool = Listeners<T>.pool[observed.id];
            
            if (pool.Count > 0) {
                pool.Insert(0, listener);
            } else {
                pool.Add(listener);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListenerFirst<T>(this Observed<T> observed, Action<T> listener, UnloadPool unload) {
            List<Action<T>> pool = Listeners<T>.pool[observed.id];
            
            if (pool.Count > 0) {
                pool.Insert(0, listener);
            } else {
                pool.Add(listener);
            }
            
            unload.Add(new UnloadAction(() => pool.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListenerLast<T>(this Observed<T> observed, Action listener) {
            List<Action> pool = Listeners.pool[observed.id];
            
            if (pool.Count > 0) {
                pool.Insert(pool.Count - 1, listener);
            } else {
                pool.Add(listener);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListenerLast<T>(this Observed<T> observed, Action listener, UnloadPool unload) {
            List<Action> pool = Listeners.pool[observed.id];
            
            if (pool.Count > 0) {
                pool.Insert(pool.Count - 1, listener);
            } else {
                pool.Add(listener);
            }
            
            unload.Add(new UnloadAction(() => pool.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListenerLast<T>(this Observed<T> observed, Action<T> listener) {
            List<Action<T>> pool = Listeners<T>.pool[observed.id];
            
            if (pool.Count > 0) {
                pool.Insert(pool.Count - 1, listener);
            } else {
                pool.Add(listener);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListenerLast<T>(this Observed<T> observed, Action<T> listener, UnloadPool unload) {
            List<Action<T>> pool = Listeners<T>.pool[observed.id];
            
            if (pool.Count > 0) {
                pool.Insert(pool.Count - 1, listener);
            } else {
                pool.Add(listener);
            }
            
            unload.Add(new UnloadAction(() => pool.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListener<T>(this Observed<T> observed, Action listener) {
            Listeners.pool[observed.id].Remove(listener);
        }
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListener<T>(this Observed<T> observed, Action<T> listener) {
            Listeners<T>.pool[observed.id].Remove(listener);
        }
        
    #endregion
    }
}