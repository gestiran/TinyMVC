using System;
using JetBrains.Annotations;
using TinyMVC.Loop;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ObservedExtension {
        public static Observed<T> AddListener<T>(this Observed<T> observed, Action listener, float frequency) {
            observed.listeners.Add(LazyListener<T>.New(listener, frequency));
            return observed;
        }

        public static Observed<T> AddListener<T>(this Observed<T> observed, Action<T> listener, float frequency) {
            observed.listeners.Add(LazyListener<T>.New(listener, frequency));
            return observed;
        }

        public static Observed<T> AddListener<T>(this Observed<T> observed, Action listener, float frequency, UnloadPool pool) {
            observed.listeners.Add(LazyListener<T>.New(listener, frequency));
            pool.Add(new UnloadAction(() => observed.RemoveListener(listener)));
            return observed;
        }
        
        public static Observed<T> AddListener<T>(this Observed<T> observed, Action<T> listener, float frequency, UnloadPool pool) {
            observed.listeners.Add(LazyListener<T>.New(listener, frequency));
            pool.Add(new UnloadAction(() => observed.RemoveListener(listener)));
            return observed;
        }
        
        public static Observed<T> AddListener<T>(this Observed<T> observed, Action listener) {
            observed.listeners.Add(Listener<T>.New(listener));
            return observed;
        }

        public static Observed<T> AddListener<T>(this Observed<T> observed, Action<T> listener) {
            observed.listeners.Add(Listener<T>.New(listener));
            return observed;
        }

        public static Observed<T> AddListener<T>(this Observed<T> observed, Action listener, UnloadPool pool) {
            observed.listeners.Add(Listener<T>.New(listener));
            pool.Add(new UnloadAction(() => observed.RemoveListener(listener)));
            return observed;
        }
        
        public static Observed<T> AddListener<T>(this Observed<T> observed, Action<T> listener, UnloadPool pool) {
            observed.listeners.Add(Listener<T>.New(listener));
            pool.Add(new UnloadAction(() => observed.RemoveListener(listener)));
            return observed;
        }
        
        public static Observed<T> RemoveListener<T>(this Observed<T> observed, Action listener) {
            observed.listeners.RemoveListener(listener);
            return observed;
        }

        public static Observed<T> RemoveListener<T>(this Observed<T> observed, Action<T> listener) {
            observed.listeners.RemoveListener(listener);
            return observed;
        }
        
        public static void AddValue(this Observed<int> observed, int value) => observed.Set(observed.value + value);

        public static void AddValue(this Observed<int> observed, [NotNull] params int[] values) {
            int value = observed.value;

            for (int i = 0; i < values.Length; i++) {
                value += values[i];
            }

            observed.Set(value);
        }

        public static void AddValue(this Observed<float> observed, float value) => observed.Set(observed.value + value);

        public static void AddValue(this Observed<float> observed, [NotNull] float[] values) {
            float value = observed.value;

            for (int i = 0; i < values.Length; i++) {
                value += values[i];
            }

            observed.Set(value);
        }

        public static void SubtractValue(this Observed<int> observed, int value) => observed.Set(observed.value - value);

        public static void SubtractValue(this Observed<int> observed, [NotNull] params int[] values) {
            int value = observed.value;

            for (int i = 0; i < values.Length; i++) {
                value -= values[i];
            }

            observed.Set(value);
        }

        public static void SubtractValue(this Observed<float> observed, float value) => observed.Set(observed.value - value);

        public static void SubtractValue(this Observed<float> observed, [NotNull] params float[] values) {
            float value = observed.value;

            for (int i = 0; i < values.Length; i++) {
                value -= values[i];
            }

            observed.Set(value);
        }

        public static void AddValueListener<T>(this Observed<T> observed, Action<T> listener, T value, UnloadPool unload) {
            observed.AddListener(newValue => {
                if (newValue.Equals(value)) {
                    listener.Invoke(newValue);
                }
            }, unload);
        }
        
        public static void AddValueListener<T>(this Observed<T> observed, Action listener, T value, UnloadPool unload) {
            observed.AddListener(newValue => {
                if (newValue.Equals(value)) {
                    listener.Invoke();
                }
            }, unload);
        }

        public static void AddValueListener<T>(this Observed<T> observed, Action<T> listener, T value) {
            observed.AddListener(newValue => {
                if (newValue.Equals(value)) {
                    listener.Invoke(newValue);
                }
            });
        }
        
        public static void AddValueListener<T>(this Observed<T> observed, Action listener, T value) {
            observed.AddListener(newValue => {
                if (newValue.Equals(value)) {
                    listener.Invoke();
                }
            });
        }
        
        public static bool TrySet<T>(this Observed<T> observed, T value) {
            if (observed.value.Equals(value)) {
                return false;
            }
            
            observed.Set(value);
            return true;
        }
        
        public static bool TryChange<T>(this Observed<T> observed, T value) {
            if (observed.isDirty) {
                return false;
            }
            
            observed.Set(value);
            return true;
        }
    }
}