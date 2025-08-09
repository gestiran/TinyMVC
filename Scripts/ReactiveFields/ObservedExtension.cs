// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TinyMVC.Boot;
using TinyMVC.Loop;
using UnityEngine;

#if UNITASK_ENABLE
using System.Threading;
using Cysharp.Threading.Tasks;
#endif

namespace TinyMVC.ReactiveFields {
    public static class ObservedExtension {
        public static void Increment(this Observed<int> obj) => obj.Set(obj.value + 1);
        
        public static void Increment(this Observed<float> obj) => obj.Set(obj.value + 1);
        
        public static void Decrement(this Observed<int> obj) => obj.Set(obj.value - 1);
        
        public static void Decrement(this Observed<float> obj) => obj.Set(obj.value - 1);
        
        public static void AddValue(this Observed<int> obj, int value) => obj.Set(obj.value + value);
        
        public static void AddValueSilent(this Observed<int> obj, int value) => obj.SetSilent(obj.value + value);
        
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
        
        public static bool IsCurrent(this Observed<int> obj, int current) => obj.value == current;
        
        [Obsolete("Can't add nothing!", true)]
        public static void AddValue(this Observed<float> obj) {
            // Do nothing
        }
        
        public static void AddValueMax(this Observed<int> obj, int value, int maxValue) => obj.Set(Mathf.Min(obj.value + value, maxValue));
        
        public static void AddValue(this Observed<int> obj, [NotNull] params int[] values) {
            int value = obj.value;
            
            for (int i = 0; i < values.Length; i++) {
                value += values[i];
            }
            
            obj.Set(value);
        }
        
        public static void AddValue(this Observed<float> obj, float value) => obj.Set(obj.value + value);
        
        public static void AddValueIfAvailable(this Observed<float> obj, float value) {
            if (value == 0) {
                return;
            }
            
            obj.AddValue(value);
        }
        
        public static void AddValueMax(this Observed<float> obj, float value, float maxValue) => obj.Set(Mathf.Min(obj.value + value, maxValue));
        
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
        
        public static void SubtractValue(this Observed<int> obj, int value) => obj.Set(obj.value - value);
        
        public static void SubtractValueSilent(this Observed<int> obj, int value) => obj.SetSilent(obj.value - value);
        
        public static void SubtractValueIfAvailable(this Observed<int> obj, int value) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValue(value);
        }
        
        public static void SubtractValueIfAvailableLimit(this Observed<int> obj, int value, int limit = 0) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValueLimit(value, limit);
        }
        
        public static void SubtractValueLimit(this Observed<int> obj, int value, int limit = 0) {
            obj.Set(Mathf.Max(obj.value - value, limit));
        }
        
        public static void SubtractValue(this Observed<int> obj, [NotNull] params int[] values) {
            int value = obj.value;
            
            for (int i = 0; i < values.Length; i++) {
                value -= values[i];
            }
            
            obj.Set(value);
        }
        
        public static void SubtractValue(this Observed<float> obj, float value) => obj.Set(obj.value - value);
        
        public static void SubtractValueIfAvailable(this Observed<float> obj, float value) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValue(value);
        }
        
        public static void SubtractValueIfAvailableLimit(this Observed<float> obj, float value, float limit = 0) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValueLimit(value, limit);
        }
        
        public static void SubtractValueLimit(this Observed<float> obj, float value, float limit = 0) {
            obj.Set(Mathf.Max(obj.value - value, limit));
        }
        
        public static void SubtractValue(this Observed<float> obj, [NotNull] params float[] values) {
            float value = obj.value;
            
            for (int i = 0; i < values.Length; i++) {
                value -= values[i];
            }
            
            obj.Set(value);
        }
        
    #if UNITASK_ENABLE
        public static async UniTask<T> GetFirstAsync<T>(this Observed<T> obj, CancellationToken cancellation) {
            T result = default;
            bool isCompleted = false;
            
            UnloadPool unload = new UnloadPool();
            
            obj.AddListener(value =>
            {
                result = value;
                isCompleted = true;
            }, unload);
            
            try {
                await UniTask.WaitUntil(() => isCompleted, PlayerLoopTiming.Update, cancellation);
            } finally {
                unload.Unload();
            }
            
            return result;
        }
        
    #endif
        
        public static bool TrySet<T>(this Observed<T> obj, T value) {
            if (EqualityComparer<T>.Default.Equals(obj.value, value)) {
                return false;
            }
            
            obj.Set(value);
            return true;
        }
        
        public static void SubtractSecond(this Observed<TimeSpan> obj, int seconds) {
            obj.Set(obj.value.Subtract(new TimeSpan(0, 0, seconds)));
        }
        
        public static bool TrySetWhen<T>(this Observed<T> obj, T equals, T set) {
            if (EqualityComparer<T>.Default.Equals(obj.value, equals) == false) {
                return false;
            }
            
            obj.Set(set);
            return true;
        }
        
        public static bool TrySetNext(this Observed<int> obj, int value) {
            if (obj.value >= value) {
                return false;
            }
            
            obj.Set(value);
            return true;
        }
        
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
        
        public static void TrySetMin(this Observed<float> obj, params float[] values) => obj.TrySet(Mathf.Min(values));
        
        public static void TrySetMin(this Observed<int> obj, params int[] values) => obj.TrySet(Mathf.Min(values));
        
        public static void TrySetClamp(this Observed<int> obj, int value, int min, int max) => obj.TrySet(Mathf.Clamp(value, min, max));
        
        [Obsolete("Can't use without parameters!", true)]
        public static void TrySetMax(this Observed<int> obj) { }
        
        [Obsolete("Can't use without parameters!", true)]
        public static void TrySetMax(this Observed<float> obj) { }
        
        public static void TrySetMax(this Observed<float> obj, params float[] values) => obj.TrySet(Mathf.Max(values));
        
        public static void TrySetMax(this Observed<int> obj, params int[] values) => obj.TrySet(Mathf.Max(values));
        
        public static bool TrySetPrevious(this Observed<int> obj, int value) {
            if (obj.value <= value) {
                return false;
            }
            
            obj.Set(value);
            return true;
        }
        
        public static bool TrySetPrevious(this Observed<float> obj, float value) {
            if (obj.value <= value) {
                return false;
            }
            
            obj.Set(value);
            return true;
        }
        
        public static void Toggle(this Observed<bool> obj) {
            if (obj.value) {
                obj.Set(false);
            } else {
                obj.Set(true);
            }
        }
        
    #region Log
        
        public static void LogChanges<T>(this Observed<T> obj, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.Log($"{label}: {value}"), unload);
        }
        
        public static void LogChanges<T>(this Observed<T> obj, string label, UnloadPool unload) {
            obj.AddListener(value => Debug.Log($"{label}: {value}"), unload);
        }
        
        public static void LogWarningChanges<T>(this Observed<T> obj, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.LogWarning($"{label}: {value}"), unload);
        }
        
        public static void LogWarningChanges<T>(this Observed<T> obj, string label, UnloadPool unload) {
            obj.AddListener(value => Debug.LogWarning($"{label}: {value}"), unload);
        }
        
        public static void LogErrorChanges<T>(this Observed<T> obj) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.LogError($"Observed: {value}"), unload);
        }
        
        public static void LogErrorChanges<T>(this Observed<T> obj, string label) {
            ProjectContext.TryGetGlobalUnload(out UnloadPool unload);
            obj.AddListener(value => Debug.LogError($"{label}: {value}"), unload);
        }
        
        public static void LogErrorChanges<T>(this Observed<T> obj, string label, UnloadPool unload) {
            obj.AddListener(value => Debug.LogError($"{label}: {value}"), unload);
        }
        
    #endregion
    }
}