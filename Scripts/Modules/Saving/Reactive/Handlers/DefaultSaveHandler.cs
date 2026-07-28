// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Modules.Saving.Reactive.Handlers {
    public static class DefaultSaveHandler<T> {
        public static ISaveHandler<T> instance { get; private set; }
        
        static DefaultSaveHandler() => instance = new GlobalSaveHandler<T>();
        
        public static void Override(ISaveHandler<T> handler) => instance = handler;
    }
}