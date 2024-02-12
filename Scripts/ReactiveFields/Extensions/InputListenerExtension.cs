using System;
using TinyMVC.Loop;

namespace TinyMVC.ReactiveFields.Extensions {
    public static class InputListenerExtension {
    #region InputListener

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
            input.listeners.RemoveListener(listener);

            return input;
        }

    #endregion

    #region InputListener<T>

        public static InputListener<T> AddListener<T>(this InputListener<T> input, Action listener) {
            input.listeners.Add(listener);

            return input;
        }

        public static InputListener<T> AddListener<T>(this InputListener<T> input, Action listener, UnloadPool pool) {
            input.listeners.Add(listener);
            pool.Add(new UnloadAction(() => input.RemoveListener(listener)));

            return input;
        }

        public static InputListener<T> AddListener<T>(this InputListener<T> input, Action<T> listener) {
            input.listeners.Add(listener);

            return input;
        }

        public static InputListener<T> AddListener<T>(this InputListener<T> input, Action<T> listener, UnloadPool pool) {
            input.listeners.Add(listener);
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

        public static InputListener<T> RemoveListener<T>(this InputListener<T> input, Action listener) {
            input.listeners.RemoveListener(listener);

            return input;
        }

        public static InputListener<T> RemoveListener<T>(this InputListener<T> input, Action<T> listener) {
            input.listeners.RemoveListener(listener);

            return input;
        }

        public static InputListener<T> RemoveListener<T>(this InputListener<T> input, MultipleListener<T> listener) {
            input.listeners.RemoveListener(listener);

            return input;
        }

    #endregion

    #region InputListener<T1, T2>

        public static InputListener<T1, T2> AddListener<T1, T2>(this InputListener<T1, T2> input, Action listener) {
            input.listeners.Add(listener);

            return input;
        }

        public static InputListener<T1, T2> AddListener<T1, T2>(this InputListener<T1, T2> input, Action listener, UnloadPool pool) {
            input.listeners.Add(listener);
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

        public static InputListener<T1, T2> RemoveListener<T1, T2>(this InputListener<T1, T2> input, Action listener) {
            input.listeners.RemoveListener(listener);

            return input;
        }

        public static InputListener<T1, T2> RemoveListener<T1, T2>(this InputListener<T1, T2> input, Action<T1, T2> listener) {
            input.listeners.RemoveListener(listener);

            return input;
        }

    #endregion

    #region InputListener<T1, T2, T3>

        public static InputListener<T1, T2, T3> AddListener<T1, T2, T3>(this InputListener<T1, T2, T3> input, Action listener) {
            input.listeners.Add(listener);

            return input;
        }

        public static InputListener<T1, T2, T3> AddListener<T1, T2, T3>(this InputListener<T1, T2, T3> input, Action listener, UnloadPool pool) {
            input.listeners.Add(listener);
            pool.Add(new UnloadAction(() => input.RemoveListener(listener)));

            return input;
        }

        public static InputListener<T1, T2, T3> AddListener<T1, T2, T3>(this InputListener<T1, T2, T3> input, Action<T1, T2, T3> listener) {
            input.listeners.Add(listener);

            return input;
        }

        public static InputListener<T1, T2, T3> AddListener<T1, T2, T3>(this InputListener<T1, T2, T3> input, Action<T1, T2, T3> listener, UnloadPool pool) {
            input.listeners.Add(listener);
            pool.Add(new UnloadAction(() => input.RemoveListener(listener)));

            return input;
        }

        public static InputListener<T1, T2, T3> RemoveListener<T1, T2, T3>(this InputListener<T1, T2, T3> input, Action listener) {
            input.listeners.RemoveListener(listener);

            return input;
        }

        public static InputListener<T1, T2, T3> RemoveListener<T1, T2, T3>(this InputListener<T1, T2, T3> input, Action<T1, T2, T3> listener) {
            input.listeners.RemoveListener(listener);

            return input;
        }

    #endregion
    }
}