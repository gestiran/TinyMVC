using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TinyMVC.Boot;
using TinyMVC.Loop;
using UnityEngine;

namespace TinyMVC.ReactiveFields.Extensions {
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
        
        [Obsolete("Can't add nothing!", true)]
        public static void AddValue(this Observed<int> observed) {
            // Do nothing
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActive(this Observed<int> observed) => observed.value >= 0;
        
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
        public static void AddValueMax(this Observed<float> observed, float value, float maxValue) => observed.Set(Mathf.Min(observed.value + value, maxValue));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValue(this Observed<float> observed, [NotNull] float[] values) {
            float value = observed.value;
            
            for (int i = 0; i < values.Length; i++) {
                value += values[i];
            }
            
            observed.Set(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValue(this Observed<int> observed, int value) => observed.Set(observed.value - value);
        
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogChanges<T>(this Observed<T> observed, string label) {
            ProjectContext.current.TryGetGlobalUnload(out UnloadPool unload);
            
            observed.AddListener(value => Debug.Log($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogChanges<T>(this Observed<T> observed, string label, UnloadPool unload) {
            observed.AddListener(value => Debug.Log($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarningChanges<T>(this Observed<T> observed, string label) {
            ProjectContext.current.TryGetGlobalUnload(out UnloadPool unload);
            
            observed.AddListener(value => Debug.LogWarning($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarningChanges<T>(this Observed<T> observed, string label, UnloadPool unload) {
            observed.AddListener(value => Debug.LogWarning($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogErrorChanges<T>(this Observed<T> observed, string label) {
            ProjectContext.current.TryGetGlobalUnload(out UnloadPool unload);
            
            observed.AddListener(value => Debug.LogError($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogErrorChanges<T>(this Observed<T> observed, string label, UnloadPool unload) {
            observed.AddListener(value => Debug.LogError($"{label}: {value}"), unload);
        }
    }
}