using System;
using TinyMVC.Loop;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ObservedExtension {
        public static Observed<T> AddListener<T>(this Observed<T> observed, Action<T> listener) {
            observed.listeners.Add(listener);
            return observed;
        }

        public static Observed<T> RemoveListener<T>(this Observed<T> observed, Action<T> listener) {
            observed.listeners.Remove(listener);
            return observed;
        }
        
        public static Observed<T> RemoveListenerOnUnload<T>(this Observed<T> observed, Action<T> listener, UnloadPool pool) {
            pool.Add(new UnloadAction(() => observed.RemoveListener(listener)));
            return observed;
        }

        public static void AddValue(this Observed<int> observed, int value) => observed.Set(observed.value + value);

        public static void AddValue(this Observed<float> observed, float value) => observed.Set(observed.value + value);

        public static void SubtractValue(this Observed<int> observed, int value) => observed.Set(observed.value - value);

        public static void SubtractValue(this Observed<float> observed, float value) => observed.Set(observed.value - value);

        public static bool TrySet<T>(this Observed<T> observed, T value) {
            if (observed.value.Equals(value)) {
                return false;
            }
            
            observed.Set(value);
            return true;
        }
    }
}