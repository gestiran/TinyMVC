using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class ObservedExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddValue(this Observed<int> observed, int value) => observed.Set(observed.value + value);

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
    }
}