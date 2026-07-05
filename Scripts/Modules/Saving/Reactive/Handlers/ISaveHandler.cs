// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Modules.Saving.Reactive.Handlers {
    public interface ISaveHandler<T> {
        public void Save(T value, string key);
        
        public void Save(T value, string key, params string[] group);
        
        public bool TryLoad(T defaultValue, out T value, string key);
        
        public bool TryLoad(T defaultValue, out T value, string key, params string[] group);
    }
}