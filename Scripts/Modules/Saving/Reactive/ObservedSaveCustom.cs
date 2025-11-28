// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using TinyReactive.Fields;

namespace TinyMVC.Modules.Saving.Reactive {
    public class ObservedSaveCustom<T> : Observed<T> {
        private ActionListener<T> _save;
        private SaveConfig<T> _config;
        
        public ObservedSaveCustom(SaveConfig<T> config, string key) : this(config, default(T), key) { }
        
        public ObservedSaveCustom(SaveConfig<T> config, T defaultValue, string key) : base(defaultValue) {
            _value = SaveService.Load(defaultValue, key);
            _save = newValue => SaveService.Save(newValue, key);
            _config = config;
        }
        
        public ObservedSaveCustom(SaveConfig<T> config, string key, params string[] group) : this(config, default, key, group) { }
        
        public ObservedSaveCustom(SaveConfig<T> config, T defaultValue, string key, params string[] group) : base(defaultValue) {
            _value = SaveService.Load(defaultValue, key, group);
            _save = newValue => SaveService.Save(newValue, key, group);
            _config = config;
        }
        
        public override void Set(T newValue) {
            base.Set(newValue);
            
            if (_config.Invoke(newValue, out T result)) {
                _save.Invoke(result);
            }
        }
        
        public override void Unload() {
            base.Unload();
            _save = EmptySave;
            _config = EmptyConfig;
        }
        
        private void EmptySave(T newValue) { }
        
        private static bool EmptyConfig(T newValue, out T result) {
            result = newValue;
            return true;
        }
    }
}