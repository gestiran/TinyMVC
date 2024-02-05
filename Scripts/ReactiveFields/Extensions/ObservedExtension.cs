namespace TinyMVC.ReactiveFields.Extensions {
    public static class ObservedExtension {
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