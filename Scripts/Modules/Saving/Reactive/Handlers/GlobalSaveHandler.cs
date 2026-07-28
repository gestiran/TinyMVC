// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Modules.Saving.Reactive.Handlers {
    public sealed class GlobalSaveHandler<T> : ISaveHandler<T> {
        public void Save(T value, string key) {
            SaveService.Save(value, key);
        }
        
        public void Save(T value, string key, params string[] group) {
            SaveService.Save(value, key, group);
        }
        
        public bool TryLoad(T defaultValue, out T value, string key) {
            if (SaveService.TryLoad(out value, key)) {
                return true;
            }
            
            value = defaultValue;
            return false;
        }
        
        public bool TryLoad(T defaultValue, out T value, string key, params string[] group) {
            if (SaveService.TryLoad(out value, key, group)) {
                return true;
            }
            
            value = defaultValue;
            return false;
        }
    }
}