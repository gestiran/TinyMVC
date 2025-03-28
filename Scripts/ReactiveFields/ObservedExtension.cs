﻿using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TinyMVC.Boot;
using TinyMVC.Loop;
using UnityEngine;

namespace TinyMVC.ReactiveFields {
    public static class ObservedExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Increment(this Observed<int> obj) => obj.Set(obj.value + 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Increment(this Observed<float> obj) => obj.Set(obj.value + 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Decrement(this Observed<int> obj) => obj.Set(obj.value - 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Decrement(this Observed<float> obj) => obj.Set(obj.value - 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValue(this Observed<int> obj, int value) => obj.Set(obj.value + value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValueSilent(this Observed<int> obj, int value) => obj.SetSilent(obj.value + value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValueIfAvailable(this Observed<int> obj, int value) {
            if (value == 0) {
                return;
            }
            
            obj.AddValue(value);
        }
        
        [Obsolete("Can't add nothing!", true)]
        public static void AddValue(this Observed<int> obj) {
            // Do nothing
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCurrent(this Observed<int> obj, int current) => obj.value == current;
        
        [Obsolete("Can't add nothing!", true)]
        public static void AddValue(this Observed<float> obj) {
            // Do nothing
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValueMax(this Observed<int> obj, int value, int maxValue) => obj.Set(Mathf.Min(obj.value + value, maxValue));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValue(this Observed<int> obj, [NotNull] params int[] values) {
            int value = obj.value;
            
            for (int i = 0; i < values.Length; i++) {
                value += values[i];
            }
            
            obj.Set(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValue(this Observed<float> obj, float value) => obj.Set(obj.value + value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValueIfAvailable(this Observed<float> obj, float value) {
            if (value == 0) {
                return;
            }
            
            obj.AddValue(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValueMax(this Observed<float> obj, float value, float maxValue) => obj.Set(Mathf.Min(obj.value + value, maxValue));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValue(this Observed<float> obj, [NotNull] float[] values) {
            float value = obj.value;
            
            for (int i = 0; i < values.Length; i++) {
                value += values[i];
            }
            
            obj.Set(value);
        }
        
        [Obsolete("Can't subtract nothing!", true)]
        public static void SubtractValue(this Observed<int> obj) {
            // Do nothing
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValue(this Observed<int> obj, int value) => obj.Set(obj.value - value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueSilent(this Observed<int> obj, int value) => obj.SetSilent(obj.value - value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueIfAvailable(this Observed<int> obj, int value) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValue(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueIfAvailableLimit(this Observed<int> obj, int value, int limit = 0) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValueLimit(value, limit);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueLimit(this Observed<int> obj, int value, int limit = 0) {
            obj.Set(Mathf.Max(obj.value - value, limit));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValue(this Observed<int> obj, [NotNull] params int[] values) {
            int value = obj.value;
            
            for (int i = 0; i < values.Length; i++) {
                value -= values[i];
            }
            
            obj.Set(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValue(this Observed<float> obj, float value) => obj.Set(obj.value - value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueIfAvailable(this Observed<float> obj, float value) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValue(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueIfAvailableLimit(this Observed<float> obj, float value, float limit = 0) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValueLimit(value, limit);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValueLimit(this Observed<float> obj, float value, float limit = 0) {
            obj.Set(Mathf.Max(obj.value - value, limit));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubtractValue(this Observed<float> obj, [NotNull] params float[] values) {
            float value = obj.value;
            
            for (int i = 0; i < values.Length; i++) {
                value -= values[i];
            }
            
            obj.Set(value);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySet<T>(this Observed<T> obj, T value) {
            if (obj.value.Equals(value)) {
                return false;
            }
            
            obj.Set(value);
            
            return true;
        }
        
        public static void SubtractSecond(this Observed<TimeSpan> obj, int seconds) {
            obj.Set(obj.value.Subtract(new TimeSpan(0, 0, seconds)));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetWhen<T>(this Observed<T> obj, T equals, T set) {
            if (obj.value.Equals(equals) == false) {
                return false;
            }
            
            obj.Set(set);
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetNext(this Observed<int> obj, int value) {
            if (obj.value >= value) {
                return false;
            }
            
            obj.Set(value);
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetNext(this Observed<float> obj, float value) {
            if (obj.value >= value) {
                return false;
            }
            
            obj.Set(value);
            
            return true;
        }
        
        [Obsolete("Can't use without parameters!", true)]
        public static void TrySetMin(this Observed<int> obj) { }
        
        [Obsolete("Can't use without parameters!", true)]
        public static void TrySetMin(this Observed<float> obj) { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySetMin(this Observed<float> obj, params float[] values) => obj.TrySet(Mathf.Min(values));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySetMin(this Observed<int> obj, params int[] values) => obj.TrySet(Mathf.Min(values));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySetClamp(this Observed<int> obj, int value, int min, int max) => obj.TrySet(Mathf.Clamp(value, min, max));
        
        [Obsolete("Can't use without parameters!", true)]
        public static void TrySetMax(this Observed<int> obj) { }
        
        [Obsolete("Can't use without parameters!", true)]
        public static void TrySetMax(this Observed<float> obj) { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySetMax(this Observed<float> obj, params float[] values) => obj.TrySet(Mathf.Max(values));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySetMax(this Observed<int> obj, params int[] values) => obj.TrySet(Mathf.Max(values));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetPrevious(this Observed<int> obj, int value) {
            if (obj.value <= value) {
                return false;
            }
            
            obj.Set(value);
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetPrevious(this Observed<float> obj, float value) {
            if (obj.value <= value) {
                return false;
            }
            
            obj.Set(value);
            
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Toggle(this Observed<bool> obj) {
            if (obj.value) {
                obj.Set(false);
            } else {
                obj.Set(true);
            }
        }
        
    #region Log
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogChanges<T>(this Observed<T> obj, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.Log($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogChanges<T>(this Observed<T> obj, string label, UnloadPool unload) {
            obj.AddListener(value => Debug.Log($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarningChanges<T>(this Observed<T> obj, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.LogWarning($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarningChanges<T>(this Observed<T> obj, string label, UnloadPool unload) {
            obj.AddListener(value => Debug.LogWarning($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogErrorChanges<T>(this Observed<T> obj) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.LogError($"Observed: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogErrorChanges<T>(this Observed<T> obj, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.LogError($"{label}: {value}"), unload);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogErrorChanges<T>(this Observed<T> obj, string label, UnloadPool unload) {
            obj.AddListener(value => Debug.LogError($"{label}: {value}"), unload);
        }
        
    #endregion
    }
}