using System;
using TinyMVC.Loop;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class InputListenerExtension {
        public static InputListener AddListener(this InputListener input, Action listener) {
            input.listeners.Add(listener);
            return input;
        }
        
        public static InputListener AddListener(this InputListener input, Action listener, UnloadPool pool) {
            input.listeners.Add(listener);
            pool.Add(new UnloadAction(() => input.RemoveListener(listener)));
            return input;
        }

        public static InputListener RemoveListener(this InputListener input, Action listener) {
            input.listeners.Remove(listener);
            return input;
        }
        
        public static InputListener RemoveListenerOnUnload(this InputListener input, Action listener, UnloadPool pool) {
            pool.Add(new UnloadAction(() => input.RemoveListener(listener)));
            return input;
        }
        
        public static InputListener<T> AddListener<T>(this InputListener<T> input, MultipleListener<T> listener) {
            input.listeners.Add(listener);
            return input;
        }
        
        public static InputListener<T> AddListener<T>(this InputListener<T> input, MultipleListener<T> listener, UnloadPool pool) {
            input.listeners.Add(listener);
            pool.Add(new UnloadAction(() => input.RemoveListener(listener)));
            return input;
        }

        public static InputListener<T> RemoveListener<T>(this InputListener<T> input, MultipleListener<T> listener) {
            input.listeners.Remove(listener);
            return input;
        }
        
        public static InputListener<T> RemoveListenerOnUnload<T>(this InputListener<T> input, MultipleListener<T> listener, UnloadPool pool) {
            pool.Add(new UnloadAction(() => input.RemoveListener(listener)));
            return input;
        }
        
        public static InputListener<T1, T2> AddListener<T1, T2>(this InputListener<T1, T2> input, Action<T1, T2> listener) {
            input.listeners.Add(listener);
            return input;
        }
        
        public static InputListener<T1, T2> AddListener<T1, T2>(this InputListener<T1, T2> input, Action<T1, T2> listener, UnloadPool pool) {
            input.listeners.Add(listener);
            pool.Add(new UnloadAction(() => input.RemoveListener(listener)));
            return input;
        }

        public static InputListener<T1, T2> RemoveListener<T1, T2>(this InputListener<T1, T2> input, Action<T1, T2> listener) {
            input.listeners.Remove(listener);
            return input;
        }
        
        public static InputListener<T1, T2> RemoveListenerOnUnload<T1, T2>(this InputListener<T1, T2> input, Action<T1, T2> listener, UnloadPool pool) {
            pool.Add(new UnloadAction(() => input.RemoveListener(listener)));
            return input;
        }
        
        public static InputListener<T1, T2, T3> AddListener<T1, T2, T3>(this InputListener<T1, T2, T3> input, Action<T1, T2, T3> listener, UnloadPool pool) {
            input.listeners.Add(listener);
            pool.Add(new UnloadAction(() => input.RemoveListener(listener)));
            return input;
        }

        public static InputListener<T1, T2, T3> RemoveListener<T1, T2, T3>(this InputListener<T1, T2, T3> input, Action<T1, T2, T3> listener) {
            input.listeners.Remove(listener);
            return input;
        }
        
        public static InputListener<T1, T2, T3> RemoveListenerOnUnload<T1, T2, T3>(this InputListener<T1, T2, T3> input, Action<T1, T2, T3> listener, UnloadPool pool) {
            pool.Add(new UnloadAction(() => input.RemoveListener(listener)));
            return input;
        }
    }
}