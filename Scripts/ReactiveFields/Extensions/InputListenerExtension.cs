using System;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class InputListenerExtension {
        public static InputListener AddListener(this InputListener input, Action listener) {
            input.listeners.Add(listener);
            return input;
        }

        public static InputListener RemoveListener(this InputListener input, Action listener) {
            input.listeners.Remove(listener);
            return input;
        }
        
        public static InputListener<T> AddListener<T>(this InputListener<T> input, MultipleListener<T> listener) {
            input.listeners.Add(listener);
            return input;
        }

        public static InputListener<T> RemoveListener<T>(this InputListener<T> input, MultipleListener<T> listener) {
            input.listeners.Remove(listener);
            return input;
        }
        
        public static InputListener<T1, T2> AddListener<T1, T2>(this InputListener<T1, T2> input, Action<T1, T2> listener) {
            input.listeners.Add(listener);
            return input;
        }

        public static InputListener<T1, T2> RemoveListener<T1, T2>(this InputListener<T1, T2> input, Action<T1, T2> listener) {
            input.listeners.Remove(listener);
            return input;
        }
        
        public static InputListener<T1, T2, T3> AddListener<T1, T2, T3>(this InputListener<T1, T2, T3> input, Action<T1, T2, T3> listener) {
            input.listeners.Add(listener);
            return input;
        }

        public static InputListener<T1, T2, T3> RemoveListener<T1, T2, T3>(this InputListener<T1, T2, T3> input, Action<T1, T2, T3> listener) {
            input.listeners.Remove(listener);
            return input;
        }
    }
}